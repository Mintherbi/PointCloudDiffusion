using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel.Types;
using NumSharp;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PointCloudDiffusion.Utils
{
    public static class Utils
    {
        public static double[,] Point2Array(List<Point3d> lspt)
        {
            int len = lspt.Count;
            double[,] arrpt = new double[len, 3];

            for (int i = 0; i < len; i++)
            {
                arrpt[i, 0] = lspt[i].X;
                arrpt[i, 1] = lspt[i].Y;
                arrpt[i, 2] = lspt[i].Z;
            }

            return arrpt;
        }

        public static List<Point3d> Array2Point(double[,] arrpt, int len)
        {
            List<Point3d> lspt = new List<Point3d>();

            for (int i = 0; i < len; i++)
            {
                lspt.Add(new Point3d(arrpt[i, 0], arrpt[i, 1], arrpt[i, 2]));
            }
            return lspt;
        }

        public static async Task SavePointCloudAsNpy(List<List<Point3d>> pointClouds, string filePath, Action<string> onProgress = null)
        {
            await Task.Run(() =>
            {
                int batchSize = pointClouds.Count;
                int numPoints = pointClouds[0].Count;

                double[,,] data = new double[batchSize, numPoints, 3];

                for (int i = 0; i < batchSize; i++)
                {
                    for (int j = 0; j < numPoints; j++)
                    {
                        data[i, j, 0] = pointClouds[i][j].X;
                        data[i, j, 1] = pointClouds[i][j].Y;
                        data[i, j, 2] = pointClouds[i][j].Z;
                    }

                    if ((i + 1) % 10 == 0)
                    {
                        onProgress?.Invoke($"{i + 1} pointclouds processed");
                    }
                }

                var npArray = np.array(data);
                np.save(filePath, npArray);

                onProgress?.Invoke("Process Completed");
            });

        }

        ///CUDA Functions
        [DllImport(@"C:\Users\jord9\source\repos\Mintherbi\PointCloudDiffusion\x64\Debug\ParallelVectorCalculation.dll")]
        public static extern void VectorAdd(double[,] point1, double[,] point2, int len, double[,] result);

        [DllImport(@"C:\Users\jord9\source\repos\Mintherbi\PointCloudDiffusion\x64\Debug\ParallelVectorCalculation.dll")]
        public static extern void BlockVectorAdd(double[,] point1, double[,] point2, int len, double[,] result);


        public static List<PythonArg> ParseArguments(string pyFilePath)
        {
            var args = new List<PythonArg>();
            var lines = File.ReadAllLines(pyFilePath);

            var argPattern = new Regex(@"add_argument\(['""]--(?<name>[^'""]+)['""],\s*(?<options>.*?)\)");
            var typePattern = new Regex(@"type\s*=\s*(?<type>\w+)");
            var defaultPattern = new Regex(@"default\s*=\s*(?<default>[^,\)]+)");
            var choicesPattern = new Regex(@"choices\s*=\s*\[(?<choices>[^\]]+)\]");

            foreach (var line in lines)
            {
                var match = argPattern.Match(line);
                if (!match.Success) continue;

                var name = match.Groups["name"].Value;
                var options = match.Groups["options"].Value;

                var typeMatch = typePattern.Match(options);
                var defaultMatch = defaultPattern.Match(options);
                var choicesMatch = choicesPattern.Match(options);

                var arg = new PythonArg
                {
                    Name = name,
                    Type = typeMatch.Success ? typeMatch.Groups["type"].Value : "str",
                    Default = defaultMatch.Success ? defaultMatch.Groups["default"].Value.Trim() : null
                };

                if (choicesMatch.Success)
                {
                    var choiceStr = choicesMatch.Groups["choices"].Value;
                    arg.Choices = choiceStr.Split(',')
                                           .Select(s => s.Trim())
                                           .ToList();
                }

                args.Add(arg);
            }

            return args;
        }
    }

    public class PythonArg
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }
        public List<string> Choices { get; set; } = new();
        public object Value { get; set; }

    }

}

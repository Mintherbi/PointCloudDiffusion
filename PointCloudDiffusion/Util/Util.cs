using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Net.Http;

using Grasshopper.Kernel;
using Rhino.Geometry;

using NumSharp;
using Grasshopper.Kernel.Geometry;

namespace Diffusion3DPrinting.Utils
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

        public static void SavePointCloudAsNpy(List<List<Point3d>> pointClouds, string filePath)
        {
            if (pointClouds == null || pointClouds.Count == 0)
                throw new ArgumentException("Point cloud list is empty.");

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
            }

            var npArray = np.array(data);
            np.save(filePath, npArray);
        }

        ///CUDA Functions
        [DllImport(@"C:\Users\jord9\source\repos\Mintherbi\PointCloudDiffusion\x64\Debug\ParallelVectorCalculation.dll")]
        public static extern void VectorAdd(double[,] point1, double[,] point2, int len, double[,] result);

        [DllImport(@"C:\Users\jord9\source\repos\Mintherbi\PointCloudDiffusion\x64\Debug\ParallelVectorCalculation.dll")]
        public static extern void BlockVectorAdd(double[,] point1, double[,] point2, int len, double[,] result);

    }


    public class AIClient
    {
        private readonly string _pythonPath;
        private readonly string _scriptPath;

        private Process process;

        public AIClient(string _pythonPath, string _scriptPath)
        {
            this._pythonPath = _pythonPath;
            this._scriptPath = _scriptPath;
        }

        public async Task StartTraining(double lr, int epoch, double[,] pointcloud)
        {
            var hyperparams = new
            {
                learning_rate = lr,
                epochs = epoch
            };
            var data = new
            {
                data = pointcloud
            };

            string hyperparamJson = JsonSerializer.Serialize(hyperparams);
            string dataJson = JsonSerializer.Serialize(data);

            string arguments = $"\"{this._scriptPath}\" --hyperparams \"{hyperparamJson}\" --data \"{dataJson}\"";

            var psi = new ProcessStartInfo
            {
                FileName = this._pythonPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            this.process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine("[Python] " + e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine("[Python ERROR] " + e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
        }
    }
}

using NumSharp;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;


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

        /*
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

                public static object ConvertDefault(string type, string raw)
                {
                    try
                    {
                        raw = raw.Trim();

                        switch (type)
                        {
                            case "int":
                                return EvaluateIntExpression(raw);
                            case "float":
                            case "double":
                                return double.Parse(raw, CultureInfo.InvariantCulture);
                            case "bool":
                            case "eval":
                                return bool.Parse(raw);
                            default:
                                return raw;
                        }
                    }
                    catch
                    {
                        return raw;
                    }
                }

                public static int EvaluateIntExpression(string expression)
                {
                    try
                    {
                        if (expression == "float('inf'")
                            return int.MaxValue;
                        expression = expression.Replace("THOUSAND", "1000");
                        var dataTable = new System.Data.DataTable();
                        var result = dataTable.Compute(expression, null);
                        return Convert.ToInt32(result);
                    }
                    catch
                    {
                        throw new FormatException($"Unable to evaluate expression: {expression}");
                    }
                }
        */
        public static class ArgparseParser
        {
            public static List<PythonArg> ParseArguments(string pyFilePath)
            {
                var args = new List<PythonArg>();
                var lines = File.ReadAllLines(pyFilePath);

                // 1. ArgumentParser 변수 이름 찾기
                var parserNames = new List<string>();
                var parserInitPattern = new Regex(@"(?<name>\w+)\s*=\s*argparse\.ArgumentParser");

                foreach (var line in lines)
                {
                    var initMatch = parserInitPattern.Match(line);
                    if (initMatch.Success)
                        parserNames.Add(initMatch.Groups["name"].Value);
                }

                if (parserNames.Count == 0)
                    parserNames.Add("parser"); // fallback 기본값

                // 2. add_argument(...) 파싱
                foreach (var line in lines)
                {
                    foreach (var parserName in parserNames)
                    {
                        var argPattern = new Regex($@"{parserName}\.add_argument\(\s*['""]--(?<name>[^'""]+)['""],\s*(?<options>.*)\)");
                        var match = argPattern.Match(line);
                        if (!match.Success) continue;

                        string name = match.Groups["name"].Value;
                        string options = match.Groups["options"].Value;

                        var typeMatch = Regex.Match(options, @"type\s*=\s*(\w+)");
                        var defaultMatch = Regex.Match(options, @"default\s*=\s*([^,\)]+)");
                        var choicesMatch = Regex.Match(options, @"choices\s*=\s*\[(?<choices>[^\]]+)\]");

                        var arg = new PythonArg
                        {
                            Name = name,
                            Type = typeMatch.Success ? typeMatch.Groups[1].Value : "str",
                            Default = defaultMatch.Success ? defaultMatch.Groups[1].Value.Trim() : null
                        };

                        if (choicesMatch.Success)
                        {
                            var choiceStr = choicesMatch.Groups["choices"].Value;
                            arg.Choices = choiceStr.Split(',')
                                                   .Select(s => s.Trim(' ', '\'', '"'))
                                                   .ToList();
                        }

                        args.Add(arg);
                    }
                }

                return args;
            }

            public static object ConvertDefault(string type, string raw)
            {
                try
                {
                    raw = raw.Trim().Replace("THOUSAND", "1000");

                    if (raw.Contains("float('inf'"))
                        return int.MaxValue;

                    switch (type.ToLower())
                    {
                        case "int":
                            return EvaluateIntExpression(raw);
                        case "float":
                        case "double":
                            return double.Parse(raw, CultureInfo.InvariantCulture);
                        case "bool":
                        case "eval":
                            return bool.Parse(raw);
                        default:
                            return raw.Trim('\"', '\''); // 문자열 따옴표 제거
                    }
                }
                catch
                {
                    return raw;
                }
            }

            public static int EvaluateIntExpression(string expression)
            {
                try
                {
                    var dataTable = new System.Data.DataTable();
                    var result = dataTable.Compute(expression, null);
                    return Convert.ToInt32(result);
                }
                catch
                {
                    throw new FormatException($"Unable to evaluate expression: {expression}");
                }
            }
        }


        public static string ConvertWindowsPathToLinux(string windowsPath)
        {
            if (string.IsNullOrWhiteSpace(windowsPath)) return "";

            string path = windowsPath.Replace("\\", "/"); // 역슬래시 → 슬래시
            if (Regex.IsMatch(path, @"^[a-zA-Z]:"))
            {
                // 드라이브 문자 추출
                char driveLetter = char.ToLower(path[0]);
                path = path.Substring(2); // "C:" 제거
                path = $"/mnt/{driveLetter}{path}";
            }

            return path;
        }

        public static string ToCommandLineArguments(List<PythonArg> args)
        {
            var parts = new List<string>();

            foreach (var arg in args)
            {
                if (arg.Value == null)
                    continue;

                string key = $"--{arg.Name}";
                string val;

                if (arg.Value is bool b)
                {
                    val = b ? "True" : "False"; // Python 쪽 eval 타입 대응
                }
                else if (arg.Value is List<string> list)
                {
                    val = string.Join(" ", list.Select(s => s.Contains(' ') ? $"\"{s}\"" : s));
                }
                else if (arg.Value is double d)
                {
                    val = d.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    val = arg.Value.ToString();
                }

                parts.Add($"{key} {val}");
            }

            return string.Join(" ", parts);
        }
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



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PointCloudDiffusion.Client
{
    public class PyWSL
    {
        private readonly string _pythonPath;
        private readonly string _scriptPath;

        private Process process;

        //Constructor
        public PyWSL(
            string _pythonPath = @"C:\Users\jord9\anaconda3\python.exe",
            string _scriptPath = @"C:\Users\jord9\source\repos\Mintherbi\PointCloudDiffusion\DPM3D\DPM3D.py")
        {
            this._pythonPath = _pythonPath;
            this._scriptPath = _scriptPath;
        }


        public async Task StartTraining(double learning_rate, int epoch, string dataPath)
        {
            var hyperparams = new
            {
                learning_rate,
                epochs = epoch
            };

            string hyperparamJson = JsonSerializer.Serialize(hyperparams);

            string arguments = $"\"{_scriptPath}\" --hyperparams \"{hyperparamJson}\" --data_path \"{dataPath}\"";

            var psi = new ProcessStartInfo
            {
                FileName = _pythonPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            process = new Process { StartInfo = psi, EnableRaisingEvents = true };

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

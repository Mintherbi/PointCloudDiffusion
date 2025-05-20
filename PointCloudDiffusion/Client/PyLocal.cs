using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;


namespace PointCloudDiffusion.Client
{
    public class PyLocal
    {
        Process process = new Process();
        public string pythonPath { get; set; }
        public string scriptPath { get; set; }
        public string args { get; set; }

        public PyLocal(string scriptPath, string args)
        : this(PATH.pythonPath, scriptPath, args) { }

        public PyLocal(string pythonPath, string scriptPath, string args=null)
        {
            process.StartInfo.FileName = this.pythonPath;
            if (pythonPath == null)
                this.pythonPath = PATH.pythonPath;

            process.StartInfo.Arguments = $"{scriptPath} {args}";
            if (scriptPath == null)
                scriptPath = PATH.HelloWorld;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
        }

        public void Run()
        {
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine("Output: " + output);
            }
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Error: " + error);
            }
        }
    }
}

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
        Process process;
       /*
        public PyLocal(string scriptPath, string args)
        : this(PATH.pythonPath, scriptPath, args) { }
            */

        public PyLocal(string scriptPath, string args=null)
        {
            //string inlineCode = $"exec(open(r\"{scriptPath}\").read())";

            this.process = new Process();
            this.process.StartInfo.FileName = PATH.powershellPath;
            this.process.StartInfo.Arguments = $"-NoExit -Command \"python \\\"{scriptPath}\\\"\"";
            this.process.StartInfo.UseShellExecute = false;
            this.process.StartInfo.RedirectStandardOutput = false;
            this.process.StartInfo.RedirectStandardError = false;
            this.process.StartInfo.CreateNoWindow = false;
        }

        public void Run()
        {
            this.process.Start();
        }
    }
}

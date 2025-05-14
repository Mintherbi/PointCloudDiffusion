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

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using NumSharp;
using Grasshopper.Kernel.Geometry;

namespace PointCloudDiffusion
{
    public class PATH
    {
        public static string PythonPath;
        public static string projectRoot = @"..";

        public static string DPM3D => Path.Combine(projectRoot, DPM3D);
        public static string HelloWorld => Path.Combine(projectRoot, HelloWorld);

    }
}

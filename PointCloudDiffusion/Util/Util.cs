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

        ///CUDA Functions
        [DllImport(@"C:\Users\jord9\source\repos\Mintherbi\PointCloudDiffusion\x64\Debug\ParallelVectorCalculation.dll")]
        public static extern void VectorAdd(double[,] point1, double[,] point2, int len, double[,] result);

        [DllImport(@"C:\Users\jord9\source\repos\Mintherbi\PointCloudDiffusion\x64\Debug\ParallelVectorCalculation.dll")]
        public static extern void BlockVectorAdd(double[,] point1, double[,] point2, int len, double[,] result);



    }

    public class AIClient
    {
        private static readonly HttpClient client = new HttpClient();
    }
}

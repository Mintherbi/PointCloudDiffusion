using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Rhino.Geometry;

using static Diffusion3DPrinting.Utils.Utils;

namespace Diffusion3DPrinting.CUDACalculation
{
    public class SingleBlockPointMove : GH_Component
    {
        public SingleBlockPointMove()
          : base("SingleBlockPointMove", "SBPM",
              "Untilizing Single Block for Point Movement Employing CUDA",
              "BinaryNature", "ARTs Lab")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("PointList", "PL", "Point to Move", GH_ParamAccess.list);
            pManager.AddPointParameter("Point2Add", "PA", "Point to Add", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Result", "R", "Result", GH_ParamAccess.list);
        }

        [DllImport(@"C:\Users\jord9\source\repos\Mintherbi\PointCloudDiffusion\x64\Debug\ParallelVectorCalculation.dll")]
        public static extern void VectorAdd(double[,] point1, double[,] point2, int len, double[,] result);
        [DllImport(@"C:\Users\jord9\source\repos\Mintherbi\PointCloudDiffusion\x64\Debug\ParallelVectorCalculation.dll")]
        public static extern void BlockVectorAdd(double[,] point1, double[,] point2, int len, double[,] result);

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> Point1 = new List<Point3d>();
            List<Point3d> Point2 = new List<Point3d>();

            if (!DA.GetDataList(0, Point1)) { return; }
            if (!DA.GetDataList(1, Point2)) { return; }

            if (Point1.Count != Point2.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Size of two vector list is not identical");
            }
            if (Point1.Count > 1024 / 3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Size of Vector is not computable in Single Block");
            }
            int len = Point1.Count;
            double[,] result = new double[len, 3];

            Task.Run(() =>
            {
                VectorAdd(Point2Array(Point1), Point2Array(Point2), len, result);

                Rhino.RhinoApp.InvokeOnUiThread(() =>
                {
                    DA.SetDataList(0, Array2Point(result, len));

                    ExpireSolution(true);
                });
            });
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("064C1005-19F5-4C5C-A8F3-B74E37D7D5B4"); }
        }
    }
}
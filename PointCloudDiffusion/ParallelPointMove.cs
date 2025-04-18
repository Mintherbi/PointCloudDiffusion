using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PointCloudDiffusion
{
    public class ParallelPointMove : GH_Component
    {
        public ParallelPointMove()
          : base("ParallelPointMove", "PPM",
              "Point Movement Employing CUDA",
              "BinaryNature", "ARTs Lab")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("PointList", "PL", "Point to Move", GH_ParamAccess.list);
            pManager.AddPointParameter("Point2Add", "PA", "Point to Add", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Result", "R", "Result", GH_ParamAccess.list);
        }

        [DllImport("ParallelVectorCalculation", CallingConvention = CallingConvention.Cdec1)]
        public static extern void VectorAdd(double[] point1, double[] point2, double[] result);

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> Point1 = new List<Point3d>();
            List<Point3d> Point2 = new List<Point3d>();

            List<Point3d> Result = new List<Point3d>();

            if (!DA.GetDataList(0, Point1)) { return; }
            if (!DA.GetDataList(1, Point2)) { return; }

            if(Point1.Count != Point2.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Size of two vector list is not identical");
            }

            VectorAdd(Point1, Point2, Result);

            DA.SetDataList(0, Result);
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
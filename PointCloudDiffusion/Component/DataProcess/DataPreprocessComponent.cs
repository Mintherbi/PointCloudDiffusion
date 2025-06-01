using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using static PointCloudDiffusion.Utils.Utils;

namespace PointCloudDiffusion.Component.DataProcess
{
    public class DataPreprocessComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public DataPreprocessComponent()
          : base("DataInput", "DI",
              "Input Data for Learning",
              "ARTs Lab", "Preprocess")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("DataInput", "DI", "Data Input", GH_ParamAccess.list);
            pManager.AddTextParameter("SavePath", "SP", "Input path for .npy file to be saved", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPathParameter("PathSaved", "PS", "Location of .npy file", GH_ParamAccess.item);
            pManager.AddTextParameter("Progress", "P", "What is he doing now?", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<List<Point3d>> DataSet = new List<List<Point3d>>();
            string Path = null;

            if (!DA.GetDataList(0, DataSet)) { return; }
            if (!DA.GetData(1, ref Path)) { return; }

            List<string> progress = new List<string>();

            Task.Run(async () =>
            {
                await SavePointCloudAsNpy(DataSet, Path,
                    msg =>
                    {
                        Rhino.RhinoApp.InvokeOnUiThread(() =>
                        {
                            progress.Add(msg);
                            DA.SetDataList(1, progress);
                        });
                    });
            });

            DA.SetData(0, Path);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B4BC5CD3-231D-4803-8468-BB883C60FA93"); }
        }
    }
}
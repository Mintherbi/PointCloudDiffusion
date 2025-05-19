using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using PointCloudDiffusion.Models;

namespace PointCloudDiffusion.Component.Train
{
    public class AutoEncoderComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public AutoEncoderComponent()
          : base("Train", "T",
              "Input the Model you want to train and datasets",
              "ARTs Lab", "Train")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>

        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            m_attributes = new CustomUI.ButtonUIAttributes(this, "TRAIN!", TrainStart, "Start Training");
        }

        public void TrainStart()
        {
            System.Windows.Forms.MessageBox.Show("Button was clicked");
        }


        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ModelOption", "MO", "Choose Model to Run and its Option", GH_ParamAccess.item);
            pManager.AddGenericParameter("Environment", "E", "Where do you want to run this model? Local or External Server", GH_ParamAccess.item);
            pManager.AddGenericParameter("Dataset", "D", "Dataset for Training", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Process", "P", "Shows the Progress of Training", GH_ParamAccess.list);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IModel Model;


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
            get { return new Guid("D2F38E95-55BD-4B38-81AF-B782F693D0DA"); }
        }
    }
}
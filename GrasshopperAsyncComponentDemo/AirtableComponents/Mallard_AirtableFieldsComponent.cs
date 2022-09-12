using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using AirtableApiClient;

namespace Mallard2
{
    public class AirtableRecordFieldsGH : GH_Component
    {


        public AirtableRecordFieldsGH() : base("Read Airtable Records", "Airtable Read",
            "Read the Field Values for given Airtable Records", "Mallard 2", "Airtable")
        {

        }


        public override Guid ComponentGuid
        {
            // Don't copy this GUID, make a new one
            get { return new Guid("fd3bfc2e-7496-4b67-af73-284c8a70c1f3"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Records", "R", "Airtable Records from the Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Field Name", "FN", "Field Name for the Airtable Records", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Values", "V", "Field Values", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare a variable for the input String

            // Use the DA object to retrieve the data inside the first input parameter.
            // If the retieval fails (for example if there is no data) we need to abort.

            if (!DA.GetData(0, ref record)) { return; }
            if (!DA.GetData(1, ref fieldName)) { return; }

            DA.SetData(0, record.GetField(fieldName));

        }

        // initial variables
        public string errorMessageString = "No Field of that name found, double check field name and try again";
        public string fieldName = "Name";  // we start with a default fieldname just incase

        public AirtableRecord record = new AirtableRecord();

        //Logo for Component
        protected override System.Drawing.Bitmap Icon { get => GrasshopperAsyncComponentDemo.Properties.Resources.AirtableFields3; }
    }
}


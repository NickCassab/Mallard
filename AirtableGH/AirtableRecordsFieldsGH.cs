using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using AirtableApiClient;

namespace AirtableGH
{
    public class AirtableRecordFieldsGH : GH_Component
    {




        public AirtableRecordFieldsGH() : base("Airtable Record Fields", "Get the Field Values" +
            "from a list of Airtable Records for the Supplied Field Name", 
            "Retrieve a list of Fields of given Airtable Records", "Duck", "Database")
        {

        }


        public override Guid ComponentGuid
        {
            // Don't copy this GUID, make a new one
            get { return new Guid("46561e26-0c0a-4e01-a46e-4bbf7614107f"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "B", "Boolean button to refresh solution", GH_ParamAccess.item);
            pManager.AddGenericParameter("Records", "R", "Airtable Records from the Airtable Base", GH_ParamAccess.list);
            pManager.AddTextParameter("Field Name", "F", "Field Name for the Airtable Records", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            
            pManager.AddTextParameter("Error Message", "E", "Error Message string", GH_ParamAccess.item);
            pManager.AddGenericParameter("Out Records", "O", "Out Record Result string", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare a variable for the input String
            bool data = false;

            // Use the DA object to retrieve the data inside the first input parameter.
            // If the retieval fails (for example if there is no data) we need to abort.
            if (!DA.GetData(0, ref data)) {
                fieldsList.Clear();
                records.Clear();
                return; }
            if (!DA.GetDataList(1, records)) { return; }
            if (!DA.GetData(2, ref fieldName)) { return; }

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (data == false) {
                fieldsList.Clear();
                records.Clear();
                return; }

            //Populate fieldlist with field information from records list
            foreach (AirtableRecord item in records)
            {
                if (item != null)
                { fieldsList.Add(item.GetField(fieldName)); }
                else
                { fieldsList.Add(""); }
                
            }

            //if the fieldlist is not zero, return success in error message slot
            if(fieldsList.Count != 0)
            {
                errorMessageString = "Success!";
            }

            // Use the DA object to assign a new String to the first output parameter.
            DA.SetData(0, errorMessageString);
            DA.SetDataList(1, fieldsList);

            //clear stored local values
            fieldsList.Clear();
            records.Clear();
        }

        // initial variables
        public string errorMessageString = "No Field of that name found, double check field name and try again";
        public string fieldName = "Name";  // we start with a default fieldname just incase

        public List<Object> fieldsList = new List<object>();
        public List<AirtableRecord> records = new List<AirtableRecord>();

        //Logo for Component
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AirtableFields2;
    }
}


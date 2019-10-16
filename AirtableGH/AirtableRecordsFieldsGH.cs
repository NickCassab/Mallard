using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using AirtableApiClient;

namespace Airtable
{
    public class AirtableRecordFieldsGH : GH_Component
    {




        public AirtableRecordFieldsGH() : base("Airtable Record Fields", "Get the Field Values" +
            "from a list of Airtable Records for the Supplied Field Name", 
            "Retrieve a list of Fields of given Airtable Records", "Data", "Database")
        {

        }


        public override Guid ComponentGuid
        {
            // Don't copy this GUID, make a new one
            get { return new Guid("46561e26-0c0a-4e01-a46e-4bbf7614107f"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "S", "Boolean Button to Refresh Solution", GH_ParamAccess.item);
            pManager.AddGenericParameter("Airtable Records", "R", "Airtable Records from and Airtable Base", GH_ParamAccess.list);
            pManager.AddTextParameter("FieldName", "F", "Field Name for the Airtable Records", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Reverse", "R", "Reversed string", GH_ParamAccess.item);
            pManager.AddTextParameter("errorMessage", "E", "ErrorMessage string", GH_ParamAccess.item);
            pManager.AddGenericParameter("outRecords", "O", "OutRecord Result string", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare a variable for the input String
            bool data = false;
            //string baseID = null;
            //string appKey = null;
            //string tablename = null;
            //string stringID = null;

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
            //if (data.Length == 0) { return; }

            // Convert the String to a character array.
            //char[] chars = data.ToCharArray();

            // Reverse the array of character.
            //System.Array.Reverse(chars);

            //

            foreach (AirtableRecord item in records)
            {
                if (item != null)
                { fieldsList.Add(item.GetField(fieldName)); }
                else
                { fieldsList.Add(""); }
                
            }

            if(fieldsList.Count != 0)
            {
                errorMessageString = "Success!";
            }


            //

            // Use the DA object to assign a new String to the first output parameter.
            DA.SetData(0, "Ran");
            DA.SetData(1, errorMessageString);
            DA.SetDataList(2, fieldsList);
            fieldsList.Clear();
            records.Clear();
        }

        //
        public string baseID = ""; 
        public string appKey = "";  
        public string tablename = "";  
        public string stringID = ""; 
        public string errorMessageString = "No Field found, doublecheck fieldname and try again";
        public string fieldName = "Name";

        public List<Object> fieldsList = new List<object>();
        public List<AirtableRecord> records = new List<AirtableRecord>();

        public string offset = null;
        public IEnumerable<string> fieldsArray = null;
        public string filterByFormula = null;
        public int? maxRecords = null;
        public int? pageSize = null;
        public IEnumerable<Sort> sort = null;
        public string view = "Main View";
        public int b = 1;
        public AirtableListRecordsResponse response;
        //
    }
}


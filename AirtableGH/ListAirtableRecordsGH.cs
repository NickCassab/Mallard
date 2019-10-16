using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using AirtableApiClient;

namespace Airtable
{
    public class ListAirtableRecordsGH : GH_Component
    {

        public ListAirtableRecordsGH() : base("List Airtable Records", "List", 
            "Retrieve a list of Airtable Records from a specific Airtable Base", "Data", "Database")
        {

        }


        public override Guid ComponentGuid
        {
            // Don't copy this GUID, make a new one
            get { return new Guid("7ed8fe52-af4e-4b9c-92db-58b6728462eb"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "S", "Boolean Button to Refresh Solution", GH_ParamAccess.item);
            pManager.AddTextParameter("BaseID", "I", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("AppKey", "K", "Appkey for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("TableName", "N", "Name of Table in Airtable Base", GH_ParamAccess.item);
            //pManager.AddTextParameter("RecordID", "R", "ID of Record to retrieve", GH_ParamAccess.item);
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
                records.Clear();
                return; }
            if (!DA.GetData(1, ref baseID)) { return; }
            if (!DA.GetData(2, ref appKey)) { return; }
            if (!DA.GetData(3, ref tablename)) { return; }
            //if (!DA.GetData(4, ref stringID)) { return; }

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (data == false) {
                records.Clear();
                return;
            }
            //if (data.Length == 0) { return; }

            // Convert the String to a character array.
            //char[] chars = data.ToCharArray();

            // Reverse the array of character.
            //System.Array.Reverse(chars);

            //

            AirtableBase airtableBase = new AirtableBase(appKey, baseID);
            Task OutResponse = ListRecordsMethodAsync(airtableBase);
            var responseString = OutResponse.ToString();
            if (response != null) {
                if(response.Records != null) {
                    records.AddRange(response.Records.ToList());
                    errorMessageString = "success!";
                }           
            }
            //

            // Use the DA object to assign a new String to the first output parameter.
            DA.SetData(0, "Ran");
            DA.SetData(1, errorMessageString);
            DA.SetDataList(2, records);


        }

        //
        public string baseID = ""; //appO6wop75JF89Phe  // remove app name and app key
        public string appKey = "";  //keyjNDU2X6XRFOsxc  // remove app name and app key
        public string tablename = "";  // // remove app name and app key
        public string stringID = ""; //// remove app name and app key
        public string errorMessageString = "no response yet, refresh to try again";
        public string attachmentFieldName = "Name";
        public List<Object> records = new List<object>();
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

        public async Task ListRecordsMethodAsync(AirtableBase airtableBase)
        {

            Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(
                                   tablename,
                                   offset,
                                   fieldsArray,
                                   filterByFormula,
                                   maxRecords,
                                   pageSize,
                                   sort,
                                   view);

            response = await task;


            if (response.AirtableApiError.ErrorMessage != null)
            {
                // Error reporting
                errorMessageString = response.AirtableApiError.ErrorMessage;
            }
            else
            {
                // Do something with the retrieved 'records' and the 'offset'
                // for the next page of the record list.
                
            }



        }










    }
}


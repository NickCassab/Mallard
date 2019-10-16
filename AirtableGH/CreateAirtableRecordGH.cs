using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using AirtableApiClient;

namespace Airtable
{
    public class CreateAirtableRecordGH : GH_Component
    {

        public CreateAirtableRecordGH() : base("Create Airtable Record", "Create", 
            "Create an Airtable Record in a specified table in a specific base", "Data", "Database")
        {

        }


        public override Guid ComponentGuid
        {
            // Don't copy this GUID, make a new one
            get { return new Guid("aa3a58d2-1774-4b8e-8cea-c708def36bf9"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "S", "Boolean Button to Refresh Solution", GH_ParamAccess.item);
            pManager.AddTextParameter("BaseID", "I", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("AppKey", "K", "Appkey for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("TableName", "N", "Name of Table in Airtable Base", GH_ParamAccess.item);
            pManager.AddGenericParameter("Fields", "F", "Fields of new airtable record", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Reverse", "R", "Reversed string", GH_ParamAccess.item);
            pManager.AddTextParameter("errorMessage", "E", "ErrorMessage string", GH_ParamAccess.item);
            pManager.AddGenericParameter("outRecord", "O", "OutRecord Result string", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare a variable for the input String
            bool data = false;

            // Use the DA object to retrieve the data inside the first input parameter.
            // If the retieval fails (for example if there is no data) we need to abort.
            if (!DA.GetData(0, ref data)) {
                return; }
            if (!DA.GetData(1, ref baseID)) { return; }
            if (!DA.GetData(2, ref appKey)) { return; }
            if (!DA.GetData(3, ref tablename)) { return; }
            if (!DA.GetDataList(4, fieldList)) { return; }

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (data == false) {
                return;
            }

            // Create airtable base and create a record
            AirtableBase airtableBase = new AirtableBase(appKey, baseID);

            if (!fields.FieldsCollection.Any())
            {
                fields.AddField("Name", fieldList[0]);
            }


            Task OutResponse = CreateRecordMethodAsync(airtableBase);
            var responseString = OutResponse.ToString();
            if (response != null) {
                records.AddRange(response.Records.ToList());
                errorMessageString = "success!";
            }
            //

            // Use the DA object to assign a new String to the first output parameter.
            DA.SetData(0, "Ran");
            DA.SetData(1, errorMessageString);
            DA.SetData(2, OutRecord);
            fieldList.Clear();
            fields.FieldsCollection.Clear();


        }

        //
        public string baseID = ""; //appO6wop75JF89Phe
        public string appKey = "";  //keyjNDU2X6XRFOsxc
        public string tablename = "People";  // People
        public Fields fields = new Fields();
        public bool conversion = false;
        public List<AirtableAttachment> attachmentList = new List<AirtableAttachment>();
        public AirtableRecord OutRecord = null;
        public List<String> fieldList = new List<string>();

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

        public async Task CreateRecordMethodAsync(AirtableBase airtableBase)
        {


            Task<AirtableCreateUpdateReplaceRecordResponse> task = airtableBase.CreateRecord(tablename, fields, conversion);
            var response = await task;

            OutRecord = response.Record;


            if (response.AirtableApiError.ErrorMessage != null)
            {
                // Error reporting
                errorMessageString = response.AirtableApiError.ErrorMessage;
            }
            else
            {
                // Do something with the retrieved 'record'
                OutRecord = response.Record;
            }
           
        }










    }
}


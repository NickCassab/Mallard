using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using AirtableApiClient;
using Newtonsoft.Json;

namespace Mallard
{
    public class UpdateAirtableRecordGH : GH_Component
    {

        public UpdateAirtableRecordGH() : base("Update Airtable Record", "Update",
            "Update an Airtable Record in a specified table in a specific base", "Mallard", "Database")
        {

        }


        public override Guid ComponentGuid
        {
            // Don't copy this GUID, make a new one
            get { return new Guid("f17a7ac2-7695-4bc5-903e-5a30cc96a7c3"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "B", "Boolean button to refresh solution", GH_ParamAccess.item);
            pManager.AddTextParameter("Base ID", "ID", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("App Key", "K", "App Key for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Table Name", "T", "Name of Table in Airtable Base", GH_ParamAccess.item);
            pManager.AddGenericParameter("Field Names", "FN", "Field Names of existing Airtable Record", GH_ParamAccess.list);
            pManager.AddGenericParameter("Fields", "F", "new Fields of existing Airtable Record", GH_ParamAccess.list);
            pManager.AddGenericParameter("Records", "R", "Airtable Records to Update", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Error Message", "E", "Error Message string", GH_ParamAccess.item);
            pManager.AddGenericParameter("Out Record", "O", "Out Record Result string", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool data = false;
            fieldNameList.Clear();
            fieldList.Clear();
            stringID = null;
            inputRecord = null;

            // Use the DA object to retrieve the data inside the first input parameter.
            // If the retieval fails (for example if there is no data) we need to abort.
            if (!DA.GetData(0, ref data))
            {
                return;
            }
            if (!DA.GetData(1, ref baseID)) { return; }
            if (!DA.GetData(2, ref appKey)) { return; }
            if (!DA.GetData(3, ref tablename)) { return; }
            if (!DA.GetDataList(4, fieldNameList)) { return; }
            if (!DA.GetDataList(5, fieldList)) { return; }
            if (!DA.GetData(6, ref inputRecord)) { return; }

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (data == false)
            {
                return;
            }

            // Create airtable base and create a record
            AirtableBase airtableBase = new AirtableBase(appKey, baseID);
            stringID = inputRecord.Id;

            if (!fields.FieldsCollection.Any())
            {
                int i = 0;
                foreach (var fieldval in fieldList)
                {

                    if (fieldval is Grasshopper.Kernel.Types.GH_String)
                    {

                        fields.AddField(fieldNameList[i], fieldval.ToString());

                    }
                    else if (fieldval is GH_ObjectWrapper)
                    {
                        GH_ObjectWrapper wrapper = (GH_ObjectWrapper)fieldval;
                        if (wrapper.Value is Newtonsoft.Json.Linq.JArray)
                        {
                            var attList = JsonConvert.DeserializeObject<List<AirtableAttachment>>(wrapper.Value.ToString());
                            fields.AddField(fieldNameList[i], attList);
                        }
                        else
                        {
                            AirtableRecord record = (AirtableRecord)wrapper.Value;

                            string recID = record.Id;

                            string[] recIDs = new string[1];
                            recIDs[0] = recID;

                            fields.AddField(fieldNameList[i], recIDs);
                        }
                        


                    }


                    i++;
                }

            }


            Task OutResponse = UpdateRecordMethodAsync(airtableBase);
            var responseString = OutResponse.ToString();

            //
            if (OutRecord != null)
            {

                errorMessageString = "Success!";

            }

            // Use the DA object to assign a new String to the first output parameter.
            DA.SetData(0, errorMessageString);
            DA.SetData(1, OutRecord);
            fieldList.Clear();
            fields.FieldsCollection.Clear();
            stringID = null;
            inputRecord = null;
        }

        //
        public string baseID = ""; 
        public string appKey = "";  
        public string tablename = null;  
        public Fields fields = new Fields();
        public bool conversion = false;
        public List<AirtableAttachment> attachmentList = new List<AirtableAttachment>();
        public AirtableRecord OutRecord = null;
        public List<String> fieldNameList = new List<string>();
        public List<Object> fieldList = new List<Object>();
        public string stringID = null;
        public AirtableRecord inputRecord = null;

        public string errorMessageString = "No response yet, refresh to try again";
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

        public async Task UpdateRecordMethodAsync(AirtableBase airtableBase)
        {


            Task<AirtableCreateUpdateReplaceRecordResponse> task = airtableBase.UpdateRecord(tablename, fields, stringID, conversion);
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

        protected override System.Drawing.Bitmap Icon => AirtableGH.Properties.Resources.AirtableUpdate2;

    }
}


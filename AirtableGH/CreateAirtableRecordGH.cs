using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using AirtableApiClient;
using Grasshopper.Kernel.Types;

namespace AirtableGH
{
    public class CreateAirtableRecordGH : GH_Component
    {

        public CreateAirtableRecordGH() : base("Create Airtable Record", "Create",
            "Create an Airtable Record in a specified table in a specific base", "Duck", "Database")
        {

        }


        public override Guid ComponentGuid
        {
            // Don't copy this GUID, make a new one
            get { return new Guid("aa3a58d2-1774-4b8e-8cea-c708def36bf9"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "B", "Boolean button to refresh solution", GH_ParamAccess.item);
            pManager.AddTextParameter("Base ID", "ID", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("App Key", "K", "App Key for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Table Name", "T", "Name of table in Airtable Base", GH_ParamAccess.item);
            pManager.AddGenericParameter("Field Names", "FN", "Field Names of new Airtable Records", GH_ParamAccess.list);
            pManager.AddGenericParameter("Fields", "F", "Fields of new Airtable Records", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Error Message", "E", "Error Message string", GH_ParamAccess.item);
            pManager.AddGenericParameter("Out Record", "O", "Out Record Result string", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare a variable for the input String
            bool data = false;
            fieldNameList.Clear();
            fieldList.Clear();

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

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (data == false)
            {
                return;
            }

            // Create airtable base and create a record
            AirtableBase airtableBase = new AirtableBase(appKey, baseID);

            if (!fields.FieldsCollection.Any())
            {
                int i = 0;
                foreach (var fieldval in fieldList)
                {
                    bool a = false;
                    if (fieldval is Grasshopper.Kernel.Types.GH_String)
                    {
                        a = true;
                        fields.AddField(fieldNameList[i], fieldval.ToString());

                    }
                    else if (fieldval is GH_ObjectWrapper)
                    {
                        GH_ObjectWrapper wrapper = (GH_ObjectWrapper)fieldval;
                        AirtableRecord record = (AirtableRecord)wrapper.Value;

                        string recID = record.Id;

                        string[] recIDs = new string[1];
                        recIDs[0] = recID;

                        fields.AddField(fieldNameList[i], recIDs);

                        a = false;
                    }


                    i++;
                }

            }


            Task OutResponse = CreateRecordMethodAsync(airtableBase);
            var responseTest = OutResponse;
            if (OutRecord != null)
            {

                errorMessageString = "Success!";

            }

            // Use the DA object to assign a new String to the first output parameter.
            DA.SetData(0, errorMessageString);
            DA.SetData(1, OutRecord);
            fieldList.Clear();
            fields.FieldsCollection.Clear();
            fieldNameList.Clear();
        }

        //
        public string baseID = "";
        public string appKey = "";
        public string tablename = "People";
        public Fields fields = new Fields();
        public bool conversion = false;
        public List<AirtableAttachment> attachmentList = new List<AirtableAttachment>();
        public AirtableRecord OutRecord = null;
        public List<Object> fieldList = new List<Object>();
        public List<String> fieldNameList = new List<string>();

        public string errorMessageString = "No response yet, refresh to try again";
        public string attachmentFieldName = "Name";
        public string FieldName = "Name";
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
                errorMessageString = response.AirtableApiError.DetailedErrorMessage2;

            }
            else
            {
                // Do something with the retrieved 'record'
                errorMessageString = "Success!";
                OutRecord = response.Record;

            }

            if (response.Success == true)
            {
                errorMessageString = "Success!";
            }

        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.AirtableCreate2;

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using AirtableApiClient;

namespace Mallard
{
    public class GetAirtableRecordGH : GH_Component
    {
                     
        public GetAirtableRecordGH() : base("Get Airtable Record", "Get", 
            "This is a Get component that retrieves a specific Airtable Record from a specific Base" +
            "and a specific Table", "Mallard", "Database")
        {

        }


        public override Guid ComponentGuid
        {
            // Don't copy this GUID, make a new one
            get { return new Guid("83d46c99-d359-4ba2-9fb5-4c5dc1cad236"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "B", "Boolean button to refresh solution", GH_ParamAccess.item);
            pManager.AddTextParameter("Base ID", "ID", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("App Key", "K", "App Key for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Table Name", "T", "Name of table in Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Record ID", "R", "ID of Record to retrieve", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Error Message", "E", "Error Message string", GH_ParamAccess.item);
            pManager.AddGenericParameter("Out Record", "O", "Out Record Result string", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare a variable for the input String
            bool data = false;

            // Use the DA object to retrieve the data inside the first input parameter.
            // If the retieval fails (for example if there is no data) we need to abort.
            if (!DA.GetData(0, ref data)) { return; }
            if (!DA.GetData(1, ref baseID)) { return; }
            if (!DA.GetData(2, ref appKey)) { return; }
            if (!DA.GetData(3, ref tablename)) { return; }
            if (!DA.GetDataList(4, stringIDs)) { return; }

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (data == false) {
                stringIDs.Clear();
                return; }

            //
            var looped = LoopTasks();
            List<String> strings = new List<string>();
            int d = 0;
            //

            while(!outRecords.Any())
            {
                if(!stringIDs.Any())
                {
                    break;
                }
                d++;
                if(d == 100000000)
                {
                    break;
                }
            }

            // Use the DA object to assign a new String to the first output parameter.
            DA.SetData(0, errorMessage);
            DA.SetDataList(1, outRecords);
            outRecords.Clear();

        }

        //
        public string baseID = "";
        public string appKey = ""; 
        public string tablename = ""; 
        public List<String> stringIDs = new List<string>(); 
        public string errorMessage = "No response yet, refresh to try again";
        public string attachmentFieldName = "Name";
        public List<AirtableRecord> outRecords = new List<AirtableRecord>();
        public AirtableRecord outRecord;
        public AirtableRetrieveRecordResponse response;
        //

        public async Task<bool> LoopTasks()
        {
            AirtableBase airtableBase = new AirtableBase(appKey, baseID);
            var tasks = new List<Task<AirtableRetrieveRecordResponse>>();

            foreach (string stringID in stringIDs)
            {
                if (stringID != null)
                {
                    Task<AirtableRetrieveRecordResponse> task = airtableBase.RetrieveRecord(tablename, stringID);
                    response = await task;
                    outRecords.Add(response.Record);
                    errorMessage = "Success!";

                } else
                {
                    outRecords.Add(null);
                    errorMessage = response.AirtableApiError.DetailedErrorMessage2;
                }           
            }
            return true;
        }

        public async Task<AirtableRetrieveRecordResponse> GetRecordMethodAsync(AirtableBase airtableBase, String stringIDparam)
        {
            Task<AirtableRetrieveRecordResponse> task = airtableBase.RetrieveRecord(tablename, stringIDparam);
            response = await task;
            return response;
        }

        protected override System.Drawing.Bitmap Icon => AirtableGH.Properties.Resources.AirtableGet2;
                       
    }
}


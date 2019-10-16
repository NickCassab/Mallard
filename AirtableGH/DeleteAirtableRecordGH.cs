using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using AirtableApiClient;

namespace Airtable
{
    public class DeleteAirtableRecordGH : GH_Component
    {
                     
        public DeleteAirtableRecordGH() : base("Delete Airtable Record", "Delete",
            "This is a Delete component that Deletes a record with a specific ID in a specific table",
            "Duck", "Database")
        {

        }


        public override Guid ComponentGuid
        {
            // Don't copy this GUID, make a new one
            get { return new Guid("543e3ad9-2618-4337-9347-7dc782a8e19d"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "S", "Boolean Button to Refresh Solution", GH_ParamAccess.item);
            pManager.AddTextParameter("BaseID", "I", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("AppKey", "K", "Appkey for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("TableName", "N", "Name of Table in Airtable Base", GH_ParamAccess.item);
            pManager.AddGenericParameter("RecordID", "R", "ID of Record to Delete", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Reverse", "R", "Reversed string", GH_ParamAccess.item);
            pManager.AddTextParameter("errorMessage", "E", "ErrorMessage string", GH_ParamAccess.item);
            pManager.AddGenericParameter("outRecord", "O", "OutRecord Result string", GH_ParamAccess.list);
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
            if (!DA.GetData(0, ref data)) { return; }
            if (!DA.GetData(1, ref baseID)) { return; }
            if (!DA.GetData(2, ref appKey)) { return; }
            if (!DA.GetData(3, ref tablename)) { return; }
            if (!DA.GetDataList(4, airtableRecordsIN)) { return; }

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (data == false) {
                airtableRecordsIN.Clear();
                //outRecords.Clear();
                return; }

            if (airtableRecordsIN.Any())
            {
                foreach (AirtableRecord record in airtableRecordsIN)
                {
                    if(record != null)
                    {
                        stringIDs.Add(record.Id);
                    }

                }
            }

            //
            var looped = LoopTasks();
            List<String> strings = new List<string>();
            int d = 0;
            //


            strings.Add("hello");

            while(!airtableRecordsIN.Any())
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
            DA.SetData(0, "Ran");
            DA.SetData(1, d.ToString());
            DA.SetDataList(2, airtableRecordsIN);
            airtableRecordsIN.Clear();
        }

        //
        public string baseID = "";
        public string appKey = "";
        public string tablename = "";  // People
        public List<String> stringIDs = new List<string>();
        public string errorMessage = "no error";
        public string attachmentFieldName = "Name";
        public List<AirtableRecord> airtableRecordsIN = new List<AirtableRecord>();
        public AirtableRecord outRecord;
        //public string stringID;
        public AirtableDeleteRecordResponse response;
        //

        public async Task<bool> LoopTasks()
        {
            AirtableBase airtableBase = new AirtableBase(appKey, baseID);
            var tasks = new List<Task<AirtableRetrieveRecordResponse>>();

            foreach (string stringID in stringIDs)
            {
                if (stringID != null)
                {
                    Task<AirtableDeleteRecordResponse> task = airtableBase.DeleteRecord(tablename, stringID);
                    response = await task;
                } else
                {
                }

            }
            //foreach (var task in await Task.WhenAll(tasks))
            //{
            //    if (task.Success)
            //    {
            //        outRecords.Add(task.Record);
            //    }
            //    else
            //    {
            //        outRecords.Add(null);
            //    }

            //}


            return true;
        }
        
    }
}


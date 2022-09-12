using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrasshopperAsyncComponent;
using System.Windows.Forms;
using AirtableApiClient;
using Newtonsoft.Json;
using System;

namespace Mallard2
{
    public class Mallard_AirtableDeleteComponent : GH_AsyncComponent
    {

        public override Guid ComponentGuid { get => new Guid("ae8fff5b-99fc-4fcf-bd95-4cfe0cb53af6"); }

        protected override System.Drawing.Bitmap Icon { get => GrasshopperAsyncComponentDemo.Properties.Resources.AirtableDelete3; }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public Mallard_AirtableDeleteComponent() : base("Delete Airtable Records", "Airtable Delete", "Deletes an airtable record with a specific ID in a specific table", "Mallard 2", "Airtable")
        {
            BaseWorker = new MallardAirtableDeleteWorker();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "B", "Boolean button to refresh solution", GH_ParamAccess.item);
            pManager.AddTextParameter("Base ID", "ID", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("App Key", "K", "App Key for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Table Name", "T", "Name of table in Airtable Base", GH_ParamAccess.item);
            pManager.AddGenericParameter("Record Object", "R", "ID of Record to delete", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Error Message", "E", "Error Message string", GH_ParamAccess.item);
            pManager.AddGenericParameter("Out Record", "O", "Out Record Result string", GH_ParamAccess.list);
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendItem(menu, "Cancel", (s, e) =>
            {
                RequestCancellation();
            });
        }
    }

    public class MallardAirtableDeleteWorker : WorkerInstance
    {
        //Variable List
        bool toggle = false;
        public string baseID = "";
        public string appKey = "";
        public string tablename = "";  // People
        public List<String> stringIDs = new List<string>();
        public string errorMessage = "No response yet, refresh to try again";
        public string attachmentFieldName = "Name";
        public List<AirtableRecord> airtableRecordsIN = new List<AirtableRecord>();
        public AirtableRecord outRecord;
        public AirtableDeleteRecordResponse response;
        //

        public MallardAirtableDeleteWorker() : base(null) { }


        public async Task DeleteRecordsMethodAsync(AirtableBase airtableBase)
        {

            foreach (string stringID in stringIDs)
            {
                if (stringID != null)
                {
                    Task<AirtableDeleteRecordResponse> task = airtableBase.DeleteRecord(tablename, stringID);
                    response = await task;
                    if (response.Success)
                    {
                        errorMessage = "Success!";
                    }
                    else
                    {
                        errorMessage = response.AirtableApiError.ErrorMessage;
                    }

                }
                else
                {
                    errorMessage = response.AirtableApiError.ErrorMessage;
                }

            }
        }

        public override void DoWork(Action<string, double> ReportProgress, Action Done)
        {
            // 👉 Checking for cancellation!
            if (!toggle) { return; }
            if (CancellationToken.IsCancellationRequested) { return; }

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (airtableRecordsIN.Any())
            {
                foreach (AirtableRecord record in airtableRecordsIN)
                {
                    if (record != null)
                    {
                        stringIDs.Add(record.Id);
                    }

                }
            }

            AirtableBase airtableBase = new AirtableBase(appKey, baseID);
            var output = DeleteRecordsMethodAsync(airtableBase);
            if(output != null)
            {
                output.Wait();
            }
            Done();
        }

        public override WorkerInstance Duplicate() => new MallardAirtableDeleteWorker();

        public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
        {
            if (!DA.GetData(0, ref toggle)) { return; }
            if (!DA.GetData(1, ref baseID)) { return; }
            if (!DA.GetData(2, ref appKey)) { return; }
            if (!DA.GetData(3, ref tablename)) { return; }
            if (!DA.GetDataList(4, airtableRecordsIN)) { return; }
        }

        public override void SetData(IGH_DataAccess DA)
        {
            // 👉 Checking for cancellation!
            if (!toggle) { return; }
            if (CancellationToken.IsCancellationRequested) { return; }
            DA.SetData(0, errorMessage);
            DA.SetDataList(1, airtableRecordsIN);
            airtableRecordsIN.Clear();
        }
    }

}

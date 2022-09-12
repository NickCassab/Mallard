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
    public class Mallard_AirtableUpdateComponent : GH_AsyncComponent
    {

        public override Guid ComponentGuid { get => new Guid("84341894-1547-4ce0-ac6c-6e63bd7f66f6"); }

        protected override System.Drawing.Bitmap Icon { get => GrasshopperAsyncComponentDemo.Properties.Resources.AirtableUpdate3; }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public Mallard_AirtableUpdateComponent() : base("Update Airtable Records", "Airtable Update", "Updates an airtable record with a specific ID in a specific table", "Mallard 2", "Airtable")
        {
            BaseWorker = new MallardAirtableUpdateWorker();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "B", "Boolean button to refresh solution", GH_ParamAccess.item);
            pManager.AddTextParameter("Base ID", "ID", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("App Key", "K", "App Key for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Table Name", "T", "Name of table in Airtable Base", GH_ParamAccess.item);
            pManager.AddGenericParameter("Record Object", "R", "ID of Record to Update", GH_ParamAccess.item);
            pManager.AddTextParameter("Field Name", "F", "Fieldname to update", GH_ParamAccess.item);
            pManager.AddTextParameter("Field Value", "V", "Field Value to update to", GH_ParamAccess.item);
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

    public class MallardAirtableUpdateWorker : WorkerInstance
    {
        //Variable List
        bool toggle = false;
        public string baseID = "";
        public string appKey = "";
        public string tablename = "";  // People
        public List<String> stringIDs = new List<string>();
        public string errorMessage = "No response yet, refresh to try again";
        public string attachmentFieldName = "Name";
        public AirtableRecord airtableRecordsIN;
        public AirtableRecord outRecord;
        public AirtableCreateUpdateReplaceRecordResponse response;
        public string stringID;
        public Fields fields;
        public string fieldname;
        public string item;
        //

        public MallardAirtableUpdateWorker() : base(null) { }


        public async Task UpdateRecordsMethodAsync(AirtableBase airtableBase)
        {

                if (stringID != null)
                {
                    fields = new Fields();
                    fields.AddField(fieldname, item);
                    Task<AirtableCreateUpdateReplaceRecordResponse> task = airtableBase.UpdateRecord(tablename, fields, stringID, true);
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

        public override void DoWork(Action<string, double> ReportProgress, Action Done)
        {
            // 👉 Checking for cancellation!
            if (!toggle) { return; }
            if (CancellationToken.IsCancellationRequested) { return; }

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (airtableRecordsIN != null)
            {
                stringID = airtableRecordsIN.Id;
            }

            AirtableBase airtableBase = new AirtableBase(appKey, baseID);
            var output = UpdateRecordsMethodAsync(airtableBase);
            if (output != null)
            {
                output.Wait();
            }
            Done();
        }

        public override WorkerInstance Duplicate() => new MallardAirtableUpdateWorker();

        public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
        {
            if (!DA.GetData(0, ref toggle)) { return; }
            if (!DA.GetData(1, ref baseID)) { return; }
            if (!DA.GetData(2, ref appKey)) { return; }
            if (!DA.GetData(3, ref tablename)) { return; }
            if (!DA.GetData(4, ref airtableRecordsIN)) { return; }
            if (!DA.GetData(5, ref fieldname)) { return; }
            if (!DA.GetData(6, ref item)) { return; }
        }

        public override void SetData(IGH_DataAccess DA)
        {
            // 👉 Checking for cancellation!
            if (!toggle) { return; }
            if (CancellationToken.IsCancellationRequested) { return; }
            DA.SetData(0, errorMessage);
            DA.SetData(1, airtableRecordsIN);

        }
    }

}

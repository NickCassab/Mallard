using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrasshopperAsyncComponent;
using System.Windows.Forms;
using AirtableApiClient;

namespace Mallard2
{
    public class Mallard_AirtableListComponent : GH_AsyncComponent
    {

        public override Guid ComponentGuid { get => new Guid("f2cfaa92-a89b-443c-8f88-43ead2341f33"); }

        protected override System.Drawing.Bitmap Icon { get => GrasshopperAsyncComponentDemo.Properties.Resources.AirtableList3; }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public Mallard_AirtableListComponent() : base("List Airtable Records", "Airtable List", "Pulls a list of Airtable Records.", "Mallard 2", "Airtable")
        {
            BaseWorker = new MallardAirtableWorker();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Base ID", "ID", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("App Key", "K", "App Key for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Table Name", "T", "Name of table in Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("View Name", "V", "Name of View in Airtable Base", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Error Message", "E", "Error Message string", GH_ParamAccess.item);
            pManager.AddGenericParameter("Out Records", "O", "Out Record Result string", GH_ParamAccess.list);
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

    public class MallardAirtableWorker : WorkerInstance
    {
        //test
        public string baseID = "";
        public string appKey = "";
        public string tablename = "";
        public string stringID = "";
        public string errorMessageString = "Recompute Component";
        public string attachmentFieldName = "Name";
        public List<Object> records = new List<object>();
        public string offset = "0";
        public IEnumerable<string> fieldsArray = null;
        public string filterByFormula = null;
        public int? maxRecords = null;
        public int? pageSize = null;
        public IEnumerable<Sort> sort = null;
        public string view = "";
        public int b = 1;
        public AirtableListRecordsResponse response;
        public int count = 0;
        public bool data = false;

        int TheNthPrime { get; set; } = 100;
        long ThePrime { get; set; } = -1;

        public MallardAirtableWorker() : base(null) { }

        public async Task ListRecordsMethodAsync(AirtableBase airtableBase, string offset)
        {
            if (CancellationToken.IsCancellationRequested) { return; }

            do
            {
                if (CancellationToken.IsCancellationRequested) { return; }

                Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(
                       tablename,
                       offset,
                       fieldsArray,
                       filterByFormula,
                       maxRecords,
                       pageSize,
                       sort,
                       view);

                AirtableListRecordsResponse response = await task;

                task.Wait();
                errorMessageString = task.Status.ToString();

                if (response.Success)
                {
                    if (CancellationToken.IsCancellationRequested) { return; }
                    errorMessageString = "Success!";//change Error Message to success here
                    records.AddRange(response.Records.ToList());
                    offset = response.Offset;
                }
                else if (response.AirtableApiError is AirtableApiException)
                {
                    if (CancellationToken.IsCancellationRequested) { return; }
                    errorMessageString = response.AirtableApiError.ErrorMessage;
                    break;
                }
                else
                {
                    if (CancellationToken.IsCancellationRequested) { return; }
                    errorMessageString = "Unknown error";
                    break;
                }


            } while (offset != null);


        }

        public override void DoWork(Action<string, double> ReportProgress, Action Done)
        {
            // 👉 Checking for cancellation!
            if (CancellationToken.IsCancellationRequested) { return; }

            AirtableBase airtableBase = new AirtableBase(appKey, baseID);
            ReportProgress(Id, (int.Parse(offset)/10)+.1);
            ListRecordsMethodAsync(airtableBase, offset).Wait();


            Done();
        }

        public override WorkerInstance Duplicate() => new MallardAirtableWorker();

        public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
        {
            DA.GetData(0, ref baseID);
            DA.GetData(1, ref appKey);
            DA.GetData(2, ref tablename);
            DA.GetData(3, ref view);


        }

        public override void SetData(IGH_DataAccess DA)
        {
            // 👉 Checking for cancellation!
            if (CancellationToken.IsCancellationRequested) { return; }
            DA.SetData(0, errorMessageString);
            DA.SetDataList(1, records);
        }
    }

}

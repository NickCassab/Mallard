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
    public class Mallard_AirtableCreateComponent : GH_AsyncComponent
    {

        public override Guid ComponentGuid { get => new Guid("ef15ac21-0771-4a3b-af82-08072fc67ec0"); }

        protected override System.Drawing.Bitmap Icon { get => GrasshopperAsyncComponentDemo.Properties.Resources.AirtableCreate3; }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public Mallard_AirtableCreateComponent() : base("Create Airtable Records", "Airtable Create", "Adds a list of new Airtable Records in the selected Table.", "Mallard 2", "Airtable")
        {
            BaseWorker = new MallardAirtableCreateWorker();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Toggle", "T", "Upload Toggle", GH_ParamAccess.item);
            pManager.AddTextParameter("Base ID", "ID", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("App Key", "K", "App Key for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Table Name", "T", "Name of table in Airtable Base", GH_ParamAccess.item);
            pManager.AddGenericParameter("Field Names", "FN", "Field Names of new Airtable Records", GH_ParamAccess.list);
            pManager.AddGenericParameter("Fields", "F", "Fields of new Airtable Records", GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Error Message", "E", "Error Message string", GH_ParamAccess.item);
            pManager.AddGenericParameter("Out Records", "O", "Out Record Result string", GH_ParamAccess.item);
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

    public class MallardAirtableCreateWorker : WorkerInstance
    {
        //test
        public string baseID = "";
        public string appKey = "";
        public string tablename = "";
        public string stringID = "";
        public bool toggle = false;
        public string errorMessageString = "Set Refresh Input to 'True'";
        public string attachmentFieldName = "Name";
        public List<Object> records = new List<object>();
        public string offset = "0";
        public List<int> indexList = new List<int>();

        public bool conversion = false;
        public List<AirtableAttachment> attachmentList = new List<AirtableAttachment>();
        public AirtableRecord OutRecord = null;
        public Grasshopper.Kernel.Data.GH_Structure<IGH_Goo> fieldList;
        public List<String> fieldNameList = new List<string>();
        public List<IGH_Goo> fieldTreeList = new List<IGH_Goo>();
        public string item = "";
        public GH_ObjectWrapper jsonitem;


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

        public MallardAirtableCreateWorker() : base(null) { }


        // add toggle boolean as a check everywhere
        // add a "duplication" check

        public async Task CreateRecordsMethodAsync(AirtableBase airtableBase)
        {
            if (!toggle) { return; }
            if (CancellationToken.IsCancellationRequested) { return; }

            int i = 0; //fieldname counter
            int j = 0; //fieldval counter

            Fields[] fields = new Fields[fieldList.Branches.ElementAt(0).Count()];
            fieldTreeList = fieldList.Branches.ElementAt(i);
            fields[j] = new Fields();

            foreach (var fieldval in fieldTreeList)
            {
                i = 0;
                fields[j] = new Fields();

                foreach(var fieldname in fieldNameList)
                {
                    IGH_Goo fieldValue = fieldList.Branches.ElementAt(i).ElementAt(j);

                    if(fieldValue is null)
                    {
                        fields[j].AddField(fieldname, "null");

                    } 
                    else
                    {
                        string item = fieldValue.ToString();
                        fields[j].AddField(fieldname, item);

                        // multi select is of type array
                    
                        // collaborator is of type array (try converting to json object?

                        //else if (fieldValue.CastTo<GH_ObjectWrapper>(out jsonitem))
                        //{

                            //if (jsonitem.Value is Newtonsoft.Json.Linq.JArray)
                            //{
                            //    fields[j].AddField(fieldNameList[i], jsonitem.Value);
                            //}
                            //else
                            //{
                            //    AirtableRecord record = (AirtableRecord)jsonitem.Value;
                            //    string recID = record.Id;
                            //    string[] recIDs = new string[1];
                            //    recIDs[j] = recID;
                            //    fields[j].AddField(fieldNameList[i], recIDs);
                            //}
                        
                        i++;
                    }

                    
                }
                j++;



            }


            if (toggle) // use a toggle to trigger writing to the table
            {
                Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = airtableBase.CreateMultipleRecords(tablename, fields, true);
                AirtableCreateUpdateReplaceMultipleRecordsResponse response = await task;
                
                task.Wait();
                errorMessageString = task.Status.ToString();

                if (response.Success)
                {
                    if (CancellationToken.IsCancellationRequested) { return; }
                    errorMessageString = "Success!";//change Error Message to success here
                    records.AddRange(response.Records.ToList());
                }
                else if (response.AirtableApiError is AirtableApiException)
                {
                    if (CancellationToken.IsCancellationRequested) { return; }
                    errorMessageString = response.AirtableApiError.ErrorMessage + " - This component can only handle 10 records at a time";
                }
                else
                {
                    if (CancellationToken.IsCancellationRequested) { return; }
                    errorMessageString = "Unknown error";
                }
            }
        }

        public override void DoWork(Action<string, double> ReportProgress, Action Done)
        {
            // 👉 Checking for cancellation!
            if (!toggle) { return; }
            if (CancellationToken.IsCancellationRequested) { return; }
            AirtableBase airtableBase = new AirtableBase(appKey, baseID);
            var output = CreateRecordsMethodAsync(airtableBase);
            if(output != null)
            {
                output.Wait();
            }


            Done();
        }

        public override WorkerInstance Duplicate() => new MallardAirtableCreateWorker();

        public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
        {
            DA.GetData(0, ref toggle);
            DA.GetData(1, ref baseID);
            DA.GetData(2, ref appKey);
            DA.GetData(3, ref tablename);
            DA.GetDataList(4, fieldNameList);
            DA.GetDataTree(5, out fieldList);
        }

        public override void SetData(IGH_DataAccess DA)
        {
            // 👉 Checking for cancellation!
            if (!toggle) { return; }
            if (CancellationToken.IsCancellationRequested) { return; }
            DA.SetData(0, errorMessageString);
            DA.SetDataList(1, records);
        }
    }

}

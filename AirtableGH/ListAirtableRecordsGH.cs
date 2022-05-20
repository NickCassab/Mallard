using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using AirtableApiClient;

namespace Mallard
{



    public class ListAirtableRecordsGH : GH_TaskCapableComponent<ListAirtableRecordsGH.SolveResults>
    {

        //
        public string baseID = "";
        public string appKey = "";
        public string tablename = "";
        public string stringID = "";
        public string errorMessageString = "Set Refresh Input to 'True'";
        public string attachmentFieldName = "Name";
        public List<Object> records = new List<object>();
        public string offset = null;
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

       // Use 'offset' and 'pageSize' to specify the records that you want
       // to retrieve.
       // Only use a 'do while' loop if you want to get multiple pages
       // of records.

        public ListAirtableRecordsGH() : base("List Airtable Records", "List", 
            "Retrieve a list of Airtable Records from a specific Airtable Base", "Mallard", "Database")
        {

        }

        public override Guid ComponentGuid
        {
            get { return new Guid("7ed8fe52-af4e-4b9c-92db-58b6728462eb"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "B", "Boolean button to refresh solution", GH_ParamAccess.item);
            pManager.AddTextParameter("Base ID", "ID", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("App Key", "K", "App Key for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Table Name", "T", "Name of table in Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("View Name", "V", "Name of View in Airtable Base", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Error Message", "E", "Error Message string", GH_ParamAccess.item);
            pManager.AddGenericParameter("Out Records", "O", "Out Record Result string", GH_ParamAccess.list);
        }

        public class SolveResults
        {
            public int Value { get; set; }
        }

        private SolveResults ListRecordsSolve(AirtableBase airtableBase, IGH_DataAccess DA)
        {
            SolveResults result = new SolveResults();
            string offset = "0";
            Task OutResponse = this.ListRecordsMethodAsync(airtableBase, offset, DA);
            result.Value = 1;
            return result;
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (InPreSolve)
            {
                // Declare a variable for the input String
                if (!DA.GetData(0, ref data)) { return; }
                if(!data)
                {
                    records.Clear();
                }

                // Use the DA object to retrieve the data inside the first input parameter.
                // If the retieval fails (for example if there is no data) we need to abort.

                if (!DA.GetData(1, ref baseID)) { return; }
                if (!DA.GetData(2, ref appKey)) { return; }
                if (!DA.GetData(3, ref tablename)) { return; }
                if (!DA.GetData(4, ref view)) { return; }

                AirtableBase airtableBase = new AirtableBase(appKey, baseID);
                Task<SolveResults> task = Task.Run(() => ListRecordsSolve(airtableBase, DA), CancelToken);
                TaskList.Add(task);
                return;
            }

            if (!GetSolveResults(DA, out SolveResults result))
            {
                if (!DA.GetData(1, ref baseID)) { return; }
                if (!DA.GetData(2, ref appKey)) { return; }
                if (!DA.GetData(3, ref tablename)) { return; }
                if (!DA.GetData(4, ref view)) { return; }
            }

            if (result != null)
            {
                DA.SetData(0, errorMessageString);
                DA.SetDataList(1, records);
            }
        }


        //async method for listing records
        public async Task ListRecordsMethodAsync(AirtableBase airtableBase, string offset, IGH_DataAccess DA)
        {
            do
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

                AirtableListRecordsResponse response = await task;
                task.Wait();
                errorMessageString = task.Status.ToString();

                if (response.Success)
                {
                    errorMessageString = "Success!";//change Error Message to success here
                    records.AddRange(response.Records.ToList());
                    offset = response.Offset;
                }
                else if (response.AirtableApiError is AirtableApiException)
                {
                    errorMessageString = response.AirtableApiError.ErrorMessage;
                    break;
                }
                else
                {
                    errorMessageString = "Unknown error";
                    break;
                }


            } while (offset != null);

           
        }

        // component icon
        protected override System.Drawing.Bitmap Icon => AirtableGH.Properties.Resources.AirtableList2;
    }
}


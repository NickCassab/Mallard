using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Airtable
{
    public class AirtableInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "AirtableGH";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "This library was written to allow Grasshopper to connect to the easily editable" +
                    "Database website called Airtable. Use Airtable to create a spreadsheet style database" +
                    "and interact with it using the components below. https://airtable.com/" +
                    "This plug-in is open-source and licensed under the MIT License https://github.com/cassab/AirtableGH ";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("bc258e47-f6d0-4d02-94d8-4a1f1b067d2f");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}

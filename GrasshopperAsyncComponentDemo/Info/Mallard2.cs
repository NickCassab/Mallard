using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Mallard2
{
  public class AirtableComponents : GH_AssemblyInfo
  {
    public override string Name
    {
      get
      {
        return "Mallard2";
      }
    }
    public override Bitmap Icon
    {
      get
      {
        //Return a 24x24 pixel bitmap to represent this GHA library.
        return GrasshopperAsyncComponentDemo.Properties.Resources.AirtablePluginIcon2;
      }
    }
    public override string Description
    {
      get
      {
        //Return a short string describing the purpose of this GHA library.
        return "A plugin for connecting Grasshopper and Airtable in an async & less janky way.";
      }
    }
    public override Guid Id
    {
      get
      {
        return new Guid("e6e5fa05-61d2-4a82-8afa-6a05cb5718b2");
      }
    }

    public override string AuthorName
    {
      get
      {
        //Return a string identifying you or your company.
        return "Nick Cassab";
      }
    }
    public override string AuthorContact
    {
      get
      {
        //Return a string representing your preferred contact details.
        return "nickcassab.com";
      }
    }
  }
}

# AirtableGH
 A Grasshopper Plugin to connect Grasshopper to the easily editable database tool Airtable, based on the CSharp fork of the Airtable API by ngonicholas, please visit his page for more information https://github.com/ngocnicholas/airtable.net

You must compile the airtable.net dll in order to use the project, see below for more info, or see link above.

Download .NET SDK 2.1.202 or newer.

Download Airtable.net c# source files from github.com. To compile to an assembly, simply create a new project in visual studio of C# .NET Standard Class Library and add these source files to the project. Refer to this link for creating a .NET Standard class library creation project using VS 2017 or newer: https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-standard-2-0/ This link also shows how to use Manage Nuget Packages in Visual Studio to refer to the Newtonsoft.Json.dll.11.0.2.

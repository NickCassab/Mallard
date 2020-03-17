# Mallard (Airtable for Grasshopper3D)
 Mallard is a Plug-In for Grasshopper3D that connects it to the easily editable database tool: Airtable. It's based on the CSharp fork of the Airtable API by ngonicholas, please visit his page for more information https://github.com/ngocnicholas/airtable.net
 
# How to Download Plugin:

If you'd like to simply try out the very Alpha version of the Grasshopper plugin, download the zip file of the Mallard Github Repo, copy the AirtableGH.gha file and the SampleLibrary.dll file in the 'bin' folder to your Grasshopper Libraries folder, reload Rhino and Grasshopper and go!

You can also use the package manager to find Mallard, or you can download Mallard from the Food4Rhino Website: https://www.food4rhino.com/app/mallard

# How to edit the code for Mallard:

You must compile the airtable.net dll in order to contribute to the project, see below for more info, or see link above.

Download .NET SDK 2.1.202 or newer.

Download Airtable.net c# source files from https://github.com/ngocnicholas/airtable.net  To compile to an assembly, simply create a new project in visual studio of C# .NET Standard Class Library and add these source files to the project. Refer to this link for creating a .NET Standard class library creation project using VS 2017 or newer: https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-standard-2-0/ This link also shows how to use Manage Nuget Packages in Visual Studio to refer to the Newtonsoft.Json.dll.11.0.2.



# Contributors:

- _cassab : https://nickcassab.com
- Susan Wu : https://www.linkedin.com/in/susannnwu/

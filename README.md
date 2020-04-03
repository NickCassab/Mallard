# Mallard (Airtable for Grasshopper3D)
 Mallard is a Plug-In for Grasshopper3D that connects it to the easily editable database tool: Airtable (https://airtable.com/). It's based on the C# fork of the Airtable API by ngonicholas, please visit his page for more information https://github.com/ngocnicholas/airtable.net
 
# How to Use the Plug-In:
![Mallard Demo](https://66.media.tumblr.com/5dc5190431cb763543f725dcf4a6266b/fa16bbeb27087304-a9/s1280x1920/fd55927b64a8abbbf45299c716719ce54a016d6a.gif)

 
# How to Download the Plug-In:

If you'd like to simply try out the very Alpha version of the Grasshopper3D Plug-In, download the Zip folder of the Mallard Github Repo, unzip the folder, and copy the AirtableGH.gha file and the SampleLibrary.dll file in the 'bin' folder to your Grasshopper3D Libraries folder. Reload Rhino and Grasshopper and go!

You can also use the package manager Yak (https://developer.rhino3d.com/guides/yak/what-is-yak/) to find Mallard, or you can download Mallard from the Food4Rhino Website: https://www.food4rhino.com/app/mallard

# How to edit the code for Mallard:

You must compile the airtable.net dll in order to contribute to the project, see below for more info, or see link above.

Download .NET SDK 2.1.202 or newer.

Download Airtable.net c# source files from https://github.com/ngocnicholas/airtable.net  To compile to an assembly, simply create a new project in visual studio of C# .NET Standard Class Library and add these source files to the project. Refer to this link for creating a .NET Standard class library creation project using VS 2017 or newer: https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-standard-2-0/ This link also shows how to use Manage Nuget Packages in Visual Studio to refer to the Newtonsoft.Json.dll.11.0.2.

Please refer to this link if you would like to contribute edits to the Master Repo: https://gist.github.com/MarcDiethelm/7303312

Please use the Mallard Rhino Discourse Forum link for communication and questions: https://discourse.mcneel.com/t/airtable-to-grasshopper-using-mallard/92231/22



# Contributors:

- _cassab : https://nickcassab.com
- Susan Wu : https://www.linkedin.com/in/susannnwu/

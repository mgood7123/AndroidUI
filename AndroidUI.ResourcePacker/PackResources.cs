using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AndroidUI
{
    public class PackResources : Task
    {
        //List of files which we need to read with the defined format: 'propertyName:type:defaultValue' per line
        [Required]
        public ITaskItem[] ResourceDirectory { get; set; }

        //The filename where the class was generated
        [Output]
        public string ClassNameFile { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Packaging Resources...");

            Generate();

            Log.LogMessage(MessageImportance.High, "Packaged resources");

            return !Log.HasLoggedErrors;
        }

        private bool Generate()
        {
            string path = ResourceDirectory[0].GetMetadata("FullPath");
            if (Directory.Exists(path))
            {
                try
                {
                    ClassNameFile = "Resources.generated.cs";
                    StringBuilder Contents = new StringBuilder(1024);
                    Contents.Append(
@" // AndroidUI Generated Resource File

using System;
namespace AndroidUI.Resources {
    public class Resources {
");
                    bool r = ScanFiles(Contents, path);
                    Contents.Append(
@"
    }
}");
                    if (r)
                    {
                        File.Delete(ClassNameFile);
                        File.WriteAllText(ClassNameFile, Contents.ToString());
                    } else
                    {
                        Log.LogError("An Error occured while packaging resources");
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    //This logging helper method is designed to capture and display information from arbitrary exceptions in a standard way.
                    Log.LogErrorFromException(ex, showStackTrace: true);
                    return false;
                }
            }
            else
            {
                Log.LogWarning("Resource directory does not exist: '" + path + "'");
            }
            return true;
        }

        private bool ScanFiles(StringBuilder contents, string path)
        {
            Log.LogMessage(MessageImportance.High, "scanning path: " + path);

            if (!PathHasNoFiles(path, "files are not supported in the root resource directory"))
            {
                return false;
            }

            string[] dirs = Directory.GetDirectories(path);

            foreach (string d in dirs)
            {
                if (d.EndsWith("layout"))
                {
                    if (!PathHasNoDirs(d, "the layout directory does not support sub-directories"))
                    {
                        return false;
                    }

                    string[] files = Directory.GetFiles(d);
                    foreach (string f in files)
                    {
                        if (!f.EndsWith("xml"))
                        {
                            Log.LogWarning("unsupported file type: " + f);
                        }
                    }
                }
                else
                {
                    Log.LogWarning("unsupported directory: " + d);
                }
            }
            return true;
        }

        bool PathHasNoFiles(string path, string message)
        {
            string[] dirs = Directory.GetFiles(path);
            if (dirs.Length != 0)
            {
                Log.LogError(message);
                return false;
            }
            return true;
        }

        bool PathHasNoDirs(string path, string message)
        {
            string[] dirs = Directory.GetDirectories(path);
            if (dirs.Length != 0)
            {
                Log.LogError(message);
                return false;
            }
            return true;
        }
    }
}
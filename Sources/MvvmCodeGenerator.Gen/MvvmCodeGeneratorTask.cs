namespace MvvmCodeGenerator.Gen
{
    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System.IO;
    using System.Xml;

    public class MvvmCodeGeneratorTask : Task
    {
        #region Properties

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public ITaskItem SourceFile { get; set; }

        #endregion

        public override bool Execute()
        {
            if (!File.Exists(this.SourceFile.ItemSpec))
            {
                Log.LogError(null, null, null, SourceFile.ItemSpec, 0, 0, 0, 0, $"file {this.SourceFile} not found");
                return false;
            }

            try
            {
                Log.LogMessage($"Loading xml : {this.SourceFile.ItemSpec}");

                var projectFolder = Path.GetDirectoryName(this.ProjectPath);
                var path = Path.Combine(projectFolder, this.SourceFile.ItemSpec);

                Log.LogMessage($"ProjectFolder: {projectFolder}");
                Log.LogMessage($"Path : {path}");                

                Arguments arguments = new Arguments
                {
                    OutputFolderProject = projectFolder
                };

                Bootstrap.Start(path, arguments);

                Log.LogMessage("End of generation.");

                return true;
            }

            catch (XmlException xe)
            {
                Log.LogMessage("Error (xml): {0}", xe.Message);
                Log.LogError(null, null, null, SourceFile.ItemSpec, xe.LineNumber, xe.LinePosition, 0, 0, $"{xe.Message}");

                return false;
            }
            catch (Exception e)
            {
                Log.LogMessage("Error: {0}", e.Message);
                Log.LogError(null, null, null, SourceFile.ItemSpec, 0, 0, 0, 0, $"{e.Message}");
                return false;
            }
        }
    }
}
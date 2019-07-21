namespace MvvmCodeGenerator.Gen
{
    using System.IO;
    using System.Xml;

    public class MvvmCodeGeneratorTask : MvvmCodeGeneratorBaseTask
    {
        protected override bool OnExecute()
        {
            try
            {
                Log.LogMessage($"Loading xml : {this.SourceFile.ItemSpec}");

                var projectFolder = Path.GetDirectoryName(this.ProjectPath);
                var path = Path.Combine(projectFolder, this.SourceFile.ItemSpec);

                Log.LogMessage($"ProjectFolder: {projectFolder}");
                Log.LogMessage($"Path : {path}");

                Arguments arguments = new Arguments
                {
                    ProjectPath = ProjectPath,
                    OutputFolderProject = projectFolder
                };

                Bootstrap.Start(path, arguments, Log);

                Log.LogMessage("End of generation.");

                return true;
            }
            catch (XmlException xe)
            {
                Log.LogMessage("Error (xml): {0}", xe.Message);
                Log.LogError(null, null, null, SourceFile.ItemSpec, xe.LineNumber, xe.LinePosition, 0, 0, $"{xe.Message}");

                return false;
            }
        }
    }
}
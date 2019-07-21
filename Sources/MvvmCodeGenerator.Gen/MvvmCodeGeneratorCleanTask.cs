namespace MvvmCodeGenerator.Gen
{
    using System.IO;

    public class MvvmCodeGeneratorCleanTask : MvvmCodeGeneratorBaseTask
    {
        protected override bool OnExecute()
        {
            var projectFolder = Path.GetDirectoryName(this.ProjectPath);

            FileHelper.Log = Log;
            FileHelper.Clean(projectFolder, "interface.g.cs");
            FileHelper.Clean(projectFolder, "part.g.cs");

            return true;
        }
    }
}

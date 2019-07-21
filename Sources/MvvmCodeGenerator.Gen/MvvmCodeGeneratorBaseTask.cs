namespace MvvmCodeGenerator.Gen
{
    using System;
    using System.IO;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public abstract class MvvmCodeGeneratorBaseTask : Task
    {
        #region Properties

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public ITaskItem SourceFile { get; set; }

        #endregion

        public override sealed bool Execute()
        {
            if (!File.Exists(this.SourceFile.ItemSpec))
            {
                Log.LogError(null, null, null, SourceFile.ItemSpec, 0, 0, 0, 0, $"file {this.SourceFile} not found");
                return false;
            }

            try
            {
                return OnExecute();
            }
            catch (Exception e)
            {
                Log.LogMessage("Error: {0}", e.Message);
                Log.LogError(null, null, null, SourceFile.ItemSpec, 0, 0, 0, 0, $"{e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Specific work done by each concrete task.
        /// </summary>
        /// <returns><c>true</c> if the task is successful; <c>false</c> otherwise.</returns>
        protected abstract bool OnExecute();
    }
}

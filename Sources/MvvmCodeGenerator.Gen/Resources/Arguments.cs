namespace MvvmCodeGenerator.Gen
{
    /// <summary>
    /// This class represent all the build arguments needed to generate the code.
    /// </summary>
    public class Arguments
    {
        /// <summary>
        /// Gets or sets the path to the project file.
        /// </summary>
        /// <value>The path to the project file.</value>
        public string ProjectPath { get; set; }

        /// <summary>
        /// Gets or sets the output folder project to generate the class.
        /// </summary>
        /// <value>The output folder project.</value>
        public string OutputFolderProject { get; set; }
    }
}

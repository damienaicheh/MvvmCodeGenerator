namespace MvvmCodeGenerator.Gen
{
    /// <summary>
    /// Interface for all code generators.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Generate the code.
        /// </summary>
        void Generate();

        /// <summary>
        /// Clean all previous generated files.
        /// </summary>
        void CleanGeneratedFiles();
    }
}

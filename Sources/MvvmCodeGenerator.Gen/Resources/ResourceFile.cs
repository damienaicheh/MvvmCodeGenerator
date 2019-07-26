namespace MvvmCodeGenerator.Gen
{
    using System.Collections.Generic;

    /// <summary>
    /// The model that represent the ResourceFile.
    /// </summary>
    public class ResourceFile
    {
        /// <summary>
        /// Generator needed.
        /// </summary>
        public string Generator { get; set; }

        /// <summary>
        /// The list of ViewModels to generate.
        /// </summary>
        public List<ViewModel> ViewModels { get; set; } 
    }
}

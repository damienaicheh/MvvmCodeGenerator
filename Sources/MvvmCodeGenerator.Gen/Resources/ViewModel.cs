namespace MvvmCodeGenerator.Gen
{
    using System.Collections.Generic;

    /// <summary>
    /// This class represent the ViewModel defined in the MvvmCodeGenMapper.xml
    /// </summary>
    public class ViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MvvmCodeGenerator.Gen.ViewModel"/> class.
        /// </summary>
        /// <param name="isItem">If set to <c>true</c> this is an ItemViewModel.</param>
        /// <param name="key">The Key correspond to the prefix of the ViewModel.</param>
        /// <param name="baseViewModel">The Base ViewModel if it exist.</param>
        /// <param name="namespace">The namespace of the ViewModel.</param>
        /// <param name="destinationFolder">The Destination folder for the ViewModel.</param>
        public ViewModel(bool isItem, string key, string baseViewModel, string @namespace, string destinationFolder)
        {
            this.Namespace = @namespace;
            this.Key = key;
            this.Base = baseViewModel;
            this.DestinationFolder = destinationFolder;
            this.IsItem = isItem;            
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace { get; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; }

        /// <summary>
        /// Gets the base.
        /// </summary>
        /// <value>The base.</value>
        public string Base { get; }

        /// <summary>
        /// Gets the destination folder.
        /// </summary>
        /// <value>The destination folder.</value>
        public string DestinationFolder { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MvvmCodeGenerator.Gen.ViewModel"/> has base.
        /// </summary>
        /// <value><c>true</c> if has base; otherwise, <c>false</c>.</value>
        public bool HasBase => !string.IsNullOrEmpty(this.Base);

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MvvmCodeGenerator.Gen.ViewModel"/> is item.
        /// </summary>
        /// <value><c>true</c> if is item; otherwise, <c>false</c>.</value>
        public bool IsItem { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public List<Property> Properties { get; } = new List<Property>();

        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>The commands.</value>
        public List<Command> Commands { get; } = new List<Command>();
    }
}

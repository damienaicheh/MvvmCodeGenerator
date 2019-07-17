namespace MvvmCodeGenerator.Gen
{
    /// <summary>
    /// This class represent the properties define in the Resource.xml files for the ViewModels.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MvvmCodeGenerator.Gen.Property"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="comment">The comment associated with it.</param>
        /// <param name="hasGet">If set to <c>true</c> has get.</param>
        /// <param name="hasSet">If set to <c>true</c> has set.</param>
        public Property(string name, string type, string comment, bool hasGet, bool hasSet)
        {
            this.Name = name;
            this.Type = type;
            this.Comment = comment;
            this.HasGet = hasGet;
            this.HasSet = hasSet;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MvvmCodeGenerator.Gen.Property"/> has get.
        /// </summary>
        /// <value><c>true</c> if has get; otherwise, <c>false</c>.</value>
        public bool HasGet { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MvvmCodeGenerator.Gen.Property"/> has set.
        /// </summary>
        /// <value><c>true</c> if has set; otherwise, <c>false</c>.</value>
        public bool HasSet { get; }
    }
}
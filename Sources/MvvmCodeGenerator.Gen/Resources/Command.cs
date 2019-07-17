namespace MvvmCodeGenerator.Gen
{
    /// <summary>
    /// This class represent the commands define in the Resource.xml files.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MvvmCodeGenerator.Gen.Command"/> class.
        /// </summary>
        /// <param name="isAsync">If set to <c>true</c> is async.</param>
        /// <param name="name">The name of the Command.</param>
        /// <param name="parameterType">The type of parameter to pass to it.</param>
        /// <param name="comment">The comment associated to this command.</param>
        /// <param name="canExecute">If set to <c>true</c> can execute.</param>
        public Command(bool isAsync, string name, string parameterType, string comment, bool canExecute)
        {
            this.IsAsync = isAsync;
            this.Name = name;
            this.ParameterType = parameterType;
            this.Comment = comment;
            this.HasCanExecute = canExecute;
        }

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public string ParameterType { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MvvmCodeGenerator.Gen.Command"/> has parameter type.
        /// </summary>
        /// <value><c>true</c> if has parameter type; otherwise, <c>false</c>.</value>
        public bool HasParameterType => !string.IsNullOrEmpty(this.ParameterType);

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MvvmCodeGenerator.Gen.Command"/> is async.
        /// </summary>
        /// <value><c>true</c> if is async; otherwise, <c>false</c>.</value>
        public bool IsAsync { get; }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MvvmCodeGenerator.Gen.Command"/> has can execute.
        /// </summary>
        /// <value><c>true</c> if has can execute; otherwise, <c>false</c>.</value>
        public bool HasCanExecute { get; }
    }
}

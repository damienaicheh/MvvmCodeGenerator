namespace MvvmCodeGenerator.Gen
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Mvvmicro library CSharp generator.
    /// </summary>
    public class MvvmicroCSharpGenerator : CSharpGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MvvmCodeGenerator.Gen.MvvmicroCSharpGenerator"/> class.
        /// </summary>
        /// <param name="viewModels">List of ViewModels to generate.</param>
        /// <param name="arguments">The build arguments.</param>
        public MvvmicroCSharpGenerator(List<ViewModel> viewModels, Arguments arguments) : base(viewModels, arguments)
        {
        }

        /// <summary>
        /// Creates the namespace that add specific using for Mvvmicro.
        /// </summary>
        /// <param name="namespace">The namespace of the class.</param>
        protected override void CreateNamespace(string @namespace)
        {
            base.CreateNamespace(@namespace);

            this.NamespaceDeclaration = this.NamespaceDeclaration.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Mvvmicro")));
        }

        /// <summary>
        /// Gets the command namespace for Mvvmicro.
        /// </summary>
        /// <returns>The command namespace.</returns>
        protected override string GetCommandNamespace() => "Mvvmicro.RelayCommand";

        /// <summary>
        /// Gets the async command namespace for Mvvmicro.
        /// </summary>
        /// <returns>The async command namespace.</returns>
        protected override string GetAsyncCommandNamespace() => "Mvvmicro.AsyncRelayCommand";

        /// <summary>
        /// Add base ViewModel if it is required by the framework.
        /// </summary>
        /// <returns>SyntaxNodeOrToken with the ViewModel base.</returns>
        protected override SyntaxNodeOrToken AddBaseViewModel()
        {
            return this.BuildBaseClass("Mvvmicro", "ViewModelBase");
        }
    }
}

namespace MvvmCodeGenerator.Gen
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using MvvmCodeGenerator.Gen.Helpers;

    /// <summary>
    /// MvvmCross library CSharp generator.
    /// </summary>
    public class MvvmCrossGenerator : CSharpGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MvvmCodeGenerator.Gen.MvvmCrossGenerator"/> class.
        /// </summary>
        /// <param name="viewModels">List of ViewModels to generate.</param>
        /// <param name="arguments">The build arguments.</param>
        public MvvmCrossGenerator(List<ViewModel> viewModels, Arguments arguments) : base(viewModels, arguments)
        {
        }

        /// <summary>
        /// Creates the namespace that add specific using for Mvvmicro.
        /// </summary>
        /// <param name="namespace">The namespace of the class.</param>
        protected override void CreateNamespace(string @namespace)
        {
            base.CreateNamespace(@namespace);

            this.NamespaceDeclaration = this.NamespaceDeclaration.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("MvvmCross.Commands")));
        }

        /// <summary>
        /// Get the entire namespace of the command implementation.
        /// </summary>
        /// <returns>The namespace of the command.</returns>
        protected override string GetCommandNamespace() => "MvvmCross.Commands.MvxCommand";

        /// <summary>
        /// Get the entire namespace of the async command implementation.
        /// </summary>
        /// <returns>The namespace of the async command.</returns>
        protected override string GetAsyncCommandNamespace() => "MvvmCross.Commands.MvxAsyncCommand";

        /// <summary>
        /// Add base ViewModel if it is required by the framework.
        /// </summary>
        /// <returns>SyntaxNodeOrToken with the ViewModel base.</returns>
        protected override SyntaxNodeOrToken AddBaseViewModel()
        {
            return this.BuildBaseClass("MvvmCross.ViewModels", "MvxViewModel");
        }

        /// <summary>
        /// Creates the public properties for each Mvvm library.
        /// </summary>
        /// <returns>The public properties.</returns>
        /// <param name="property">The property to generate.</param>
        public override PropertyDeclarationSyntax CreatePublicProperties(Property property)
        {
            return PropertiesHelper.GeneratePropertiesSyntax(property);
        }
    }
}

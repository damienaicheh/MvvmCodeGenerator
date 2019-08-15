namespace MvvmCodeGenerator.Gen
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using MvvmCodeGenerator.Gen.Helpers;

    /// <summary>
    /// Prism library CSharp generator.
    /// </summary>
    public class PrismCoreGenerator : CSharpGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MvvmCodeGenerator.Gen.PrismCoreGenerator"/> class.
        /// </summary>
        /// <param name="viewModels">List of ViewModels to generate.</param>
        /// <param name="arguments">The build arguments.</param>
        public PrismCoreGenerator(List<ViewModel> viewModels, Arguments arguments) : base(viewModels, arguments)
        {
            this.IsRunningPropertyForCommandNeeded = true;
        }

        /// <summary>
        /// Creates the namespace that add specific using for PrismGenerator.
        /// </summary>
        /// <param name="namespace">The namespace of the class.</param>
        protected override void CreateNamespace(string @namespace)
        {
            base.CreateNamespace(@namespace);

            this.NamespaceDeclaration = this.NamespaceDeclaration.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Prism.Mvvm")));
            this.NamespaceDeclaration = this.NamespaceDeclaration.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Prism.Commands")));
        }

        /// <summary>
        /// Gets the command namespace for Prism.
        /// </summary>
        /// <returns>The command namespace.</returns>
        protected override string GetAsyncCommandNamespace() => "Prism.Commands.DelegateCommand";

        /// <summary>
        /// Gets the command namespace for Prism.
        /// </summary>
        /// <returns>The command namespace.</returns>
        protected override string GetCommandNamespace() => "Prism.Commands.DelegateCommand";

        /// <summary>
        /// Add base ViewModel if it is required by the framework.
        /// </summary>
        /// <returns>SyntaxNodeOrToken with the ViewModel base.</returns>
        protected override SyntaxNodeOrToken AddBaseViewModel()
        {
            return this.BuildBaseClass("Prism.Mvvm", "BindableBase");
        }

        /// <summary>
        /// Get command method syntax, depending on the framework used.
        /// </summary>
        /// <param name="command">The command to generate.</param>
        /// <returns>The ArgumentListSyntax that represent the command syntax to generate</returns>
        protected override ArgumentListSyntax GetCommandMethodSyntax(Command command)
        {
            return CommandHelper.GenerateSafeCommandSyntax(command);
        }

        /// <summary>
        /// Generate command comment to indicate which method associated to the command needs to be implemented.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The complete comment to the command.</returns>        
        public override string GenerateCommandComment(Command command)
        {
            var comment = base.GenerateCommandComment(command);
            return CommandHelper.AddCommandExceptionComment(command, comment);
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

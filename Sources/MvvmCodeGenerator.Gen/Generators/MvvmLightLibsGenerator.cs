namespace MvvmCodeGenerator.Gen
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// MvvmLightLibs library CSharp generator.
    /// </summary>
    public class MvvmLightLibsGenerator : CSharpGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MvvmCodeGenerator.Gen.MvvmLightLibsGenerator"/> class.
        /// </summary>
        /// <param name="viewModels">List of ViewModels to generate.</param>
        /// <param name="arguments">The build arguments.</param>
        public MvvmLightLibsGenerator(List<ViewModel> viewModels, Arguments arguments) : base(viewModels, arguments)
        {
        }

        /// <summary>
        /// Creates the namespace that add specific using for MvvmLightLibs.
        /// </summary>
        /// <param name="namespace">The namespace of the class.</param>
        protected override void CreateNamespace(string @namespace)
        {
            base.CreateNamespace(@namespace);

            this.NamespaceDeclaration = this.NamespaceDeclaration.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("GalaSoft.MvvmLight.Command")));
        }

        /// <summary>
        /// Gets the command namespace for MvvmLightLibs.
        /// </summary>
        /// <returns>The command namespace.</returns>
        protected override string GetAsyncCommandNamespace() => "GalaSoft.MvvmLight.Command.RelayCommand";

        /// <summary>
        /// Gets the command namespace for MvvmLightLibs.
        /// </summary>
        /// <returns>The command namespace.</returns>
        protected override string GetCommandNamespace() => "GalaSoft.MvvmLight.Command.RelayCommand";

        /// <summary>
        /// Add base ViewModel if it is required by the framework.
        /// </summary>
        /// <returns>SyntaxNodeOrToken with the ViewModel base.</returns>
        protected override SyntaxNodeOrToken AddBaseViewModel()
        {
            return this.BuildBaseClass("GalaSoft.MvvmLight", "ViewModelBase");
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
    }
}

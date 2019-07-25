namespace MvvmCodeGenerator.Gen
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// FreshMvvm library CSharp generator.
    /// </summary>
    public class FreshMvvmGenerator : CSharpGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MvvmCodeGenerator.Gen.FreshMvvmGenerator"/> class.
        /// </summary>
        /// <param name="viewModels">List of ViewModels to generate.</param>
        /// <param name="arguments">The build arguments.</param>
        public FreshMvvmGenerator(List<ViewModel> viewModels, Arguments arguments) : base(viewModels, arguments)
        {
            this.IsRunningPropertyForCommandNeeded = true;
        }

        /// <summary>
        /// Creates the namespace that add specific using for FreshMvvmGenerator.
        /// </summary>
        /// <param name="namespace">The namespace of the class.</param>
        protected override void CreateNamespace(string @namespace)
        {
            base.CreateNamespace(@namespace);

            this.NamespaceDeclaration = this.NamespaceDeclaration.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("FreshMvvm")));
            this.NamespaceDeclaration = this.NamespaceDeclaration.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Xamarin.Forms")));
        }

        /// <summary>
        /// Gets the command namespace for FreshMvvm.
        /// </summary>
        /// <returns>The command namespace.</returns>
        protected override string GetAsyncCommandNamespace() => "Xamarin.Forms.Command";

        /// <summary>
        /// Gets the command namespace for FreshMvvm.
        /// </summary>
        /// <returns>The command namespace.</returns>
        protected override string GetCommandNamespace() => "Xamarin.Forms.Command";

        /// <summary>
        /// Add base ViewModel if it is required by the framework.
        /// </summary>
        /// <returns>SyntaxNodeOrToken with the ViewModel base.</returns>
        protected override SyntaxNodeOrToken AddBaseViewModel()
        {
            return this.BuildBaseClass("FreshMvvm", "FreshBasePageModel");
        }

        /// <summary>
        /// Creates the public properties for each Mvvm library.
        /// </summary>
        /// <returns>The public properties.</returns>
        /// <param name="property">The property to generate.</param>
        public override PropertyDeclarationSyntax CreatePublicProperties(Property property)
        {
            return SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(property.Type.FindType()), property.Name)
               .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
               .AddAccessorListAccessors(
                   SyntaxFactory.AccessorDeclaration(
                       SyntaxKind.GetAccessorDeclaration)
                       .WithExpressionBody(
                           SyntaxFactory.ArrowExpressionClause(
                               SyntaxFactory.MemberAccessExpression(
                                   SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName(property.Name.ToCamelCase()))))
                                   .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                   SyntaxFactory.AccessorDeclaration(
                       SyntaxKind.SetAccessorDeclaration)
                       .WithBody(
                            SyntaxFactory.Block(
                                SyntaxFactory.ExpressionStatement(
                                    SyntaxFactory.AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        SyntaxFactory.IdentifierName(property.Name.ToCamelCase()),
                                        SyntaxFactory.IdentifierName("value"))),
                                SyntaxFactory.ExpressionStatement(
                                    SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.IdentifierName("RaisePropertyChanged"))))))
                       .WithModifiers(FormatterHelper.GenerateComment(property.Comment));
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

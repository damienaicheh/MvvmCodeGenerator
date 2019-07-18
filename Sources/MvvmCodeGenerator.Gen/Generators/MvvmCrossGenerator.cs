﻿namespace MvvmCodeGenerator.Gen
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                       .WithExpressionBody(
                           SyntaxFactory.ArrowExpressionClause(
                               SyntaxFactory.InvocationExpression(
                                   SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName("SetProperty")))
                                   .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                       new SyntaxNodeOrToken[]
                                       {
                                            SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName(property.Name.ToCamelCase())))
                                                .WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)), SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value"))
                                       })))))
                       .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                       .WithModifiers(FormatterHelper.GenerateComment(property.Comment));

        }

        /// <summary>
        /// Get command method syntax, depending on the framework used.
        /// </summary>
        /// <param name="command">The command to generate.</param>
        /// <returns>The ArgumentListSyntax that represent the command syntax to generate</returns>
        protected override ArgumentListSyntax GetCommandMethodSyntax(Command command)
        {
            var syntaxeNodeOrToken = new List<SyntaxNodeOrToken>();
            syntaxeNodeOrToken.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(string.Concat("Execute", command.FormatCommandName(), command.IsAsync ? "Async" : string.Empty))));

            if (command.HasCanExecute)
            {
                syntaxeNodeOrToken.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                syntaxeNodeOrToken.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("CanExecute" + command.FormatCommandName())));
            }

            return SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(syntaxeNodeOrToken));
        }
    }
}
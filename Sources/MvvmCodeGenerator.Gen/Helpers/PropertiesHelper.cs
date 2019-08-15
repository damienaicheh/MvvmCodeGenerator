namespace MvvmCodeGenerator.Gen.Helpers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// This class help to mutualize some code related to the Properties that are the same for multiple frameworks.
    /// </summary>
    public static class PropertiesHelper
    {
        /// <summary>
        /// Creates the public properties for each Mvvm library.
        /// </summary>
        /// <returns>The public properties.</returns>
        /// <param name="property">The property to generate.</param>
        public static PropertyDeclarationSyntax GeneratePropertiesSyntax(Property property)
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
    }
}

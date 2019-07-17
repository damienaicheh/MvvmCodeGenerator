namespace MvvmCodeGenerator.Gen
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// This class help to mutualize some code related to the Commands that are the same for multiple frameworks.
    /// </summary>
    public static class CommandHelper
    {
        /// <summary>
        /// Generates the safe command syntax using the try / catch to encapsulate the async commands.
        /// </summary>
        /// <returns>The safe command syntax.</returns>
        /// <param name="command">The command.</param>
        public static ArgumentListSyntax GenerateSafeCommandSyntax(Command command)
        {
            var syntaxeNodeOrToken = new List<SyntaxNodeOrToken>();

            if (command.IsAsync)
            {
                var methodExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(string.Concat("Execute", command.FormatCommandName(), "Async")));

                if (command.HasParameterType)
                {
                    methodExpression = methodExpression.WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value")))));
                }

                // Wrap the async void command to a try catch using the SafeFireAndForget extension
                var arg = SyntaxFactory.ParenthesizedLambdaExpression(
                            SyntaxFactory.Block(
                                SyntaxFactory.SingletonList<StatementSyntax>(
                                    SyntaxFactory.TryStatement(
                                        SyntaxFactory.SingletonList(
                                            SyntaxFactory.CatchClause().WithDeclaration(
                                                SyntaxFactory.CatchDeclaration(SyntaxFactory.IdentifierName("System.Exception")).WithIdentifier(SyntaxFactory.Identifier("ex")))
                                            .WithBlock(SyntaxFactory.Block(
                                                SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(command.FormatCommandExceptionName()))
                                            .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("ex")))))))))))
                                    .WithBlock(
                                        SyntaxFactory.Block(
                                            SyntaxFactory.SingletonList<StatementSyntax>(
                                                SyntaxFactory.ExpressionStatement(
                                                    SyntaxFactory.AwaitExpression(methodExpression)))))))).WithAsyncKeyword(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));

                if (command.HasParameterType)
                {
                    arg = arg.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(SyntaxFactory.Identifier("value")))));
                }

                syntaxeNodeOrToken.Add(SyntaxFactory.Argument(arg));
            }
            else
            {
                syntaxeNodeOrToken.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(string.Concat("Execute", command.FormatCommandName()))));
            }

            if (command.HasCanExecute)
            {
                syntaxeNodeOrToken.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                syntaxeNodeOrToken.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("CanExecute" + command.FormatCommandName())));
            }

            return SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(syntaxeNodeOrToken));
        }
    
        /// <summary>
        /// Add the command exception comment.
        /// </summary>
        /// <returns>The command exception comment.</returns>
        /// <param name="command">The command.</param>
        /// <param name="comment">The command comment to complete.</param>
        public static string AddCommandExceptionComment(Command command, string comment)
        {
            return string.Concat($"{comment} and {command.FormatCommandExceptionName()}");
        }
    }
}

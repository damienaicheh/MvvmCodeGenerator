namespace MvvmCodeGenerator.Gen
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Build.Utilities;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Base for all generators.
    /// </summary>
    public abstract class CSharpGenerator : IGenerator
    {
        /// <summary>
        /// Constant value for the name, wihtout extension, of the target generated file.
        /// </summary>
        private const string GeneratedTargetFileWithoutExtension = "MvvmCodeGenMapper";

        /// <summary>
        /// Constant value for the file extension of the target generated file.
        /// </summary>
        private const string GeneratedTargetFileExtension = ".g.targets";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MvvmCodeGenerator.Gen.CSharpGenerator"/> class.
        /// Constructor base for all generators.
        /// </summary>
        /// <param name="viewModels">List of ViewModels to generate.</param>
        /// <param name="arguments">The arguments console.</param>
        protected CSharpGenerator(List<ViewModel> viewModels, Arguments arguments)
        {
            this.ViewModels = viewModels;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Logger defined in the task.
        /// </summary>
        public TaskLoggingHelper Log { private get; set; }

        /// <summary>
        /// Gets or sets The namespace declaration for the current generated class.
        /// </summary>
        /// <value>The namespace declaration.</value>
        protected NamespaceDeclarationSyntax NamespaceDeclaration { get; set; }

        /// <summary>
        /// Gets or sets the current generated class declaration.
        /// </summary>
        /// <value>The class declaration.</value>
        protected ClassDeclarationSyntax ClassDeclaration { get; set; }

        /// <summary>
        /// Gets or sets the current generated interface.
        /// </summary>
        /// <value>The interface declaration.</value>
        protected InterfaceDeclarationSyntax InterfaceDeclaration { get; set; }

        private List<ViewModel> ViewModels { get; set; }

        private Arguments Arguments { get; set; }

        /// <summary>
        /// Generate the ViewModels.
        /// </summary>
        public void Generate()
        {
            foreach (var viewModel in this.ViewModels)
            {
                this.GenerateViewModelImplementationFile(viewModel);
                this.GenerateViewModelInterfaceFile(viewModel);
                this.GenerateViewModelPartialFile(viewModel);
            }

            this.GenerateTarget(ViewModels, out string generatedTargetFilename);
            this.InjectProject(generatedTargetFilename);
        }

        /// <summary>
        /// Clean previous files.
        /// </summary>
        public void CleanGeneratedFiles()
        {
            FileHelper.Clean(this.Arguments.OutputFolderProject, "interface.g.cs");
            FileHelper.Clean(this.Arguments.OutputFolderProject, "part.g.cs");
        }

        /// <summary>
        /// Get the entire namespace of the command implementation.
        /// </summary>
        /// <returns>The namespace of the command.</returns>
        protected abstract string GetCommandNamespace();

        /// <summary>
        /// Get the entire namespace of the async command implementation.
        /// </summary>
        /// <returns>The namespace of the async command.</returns>
        protected abstract string GetAsyncCommandNamespace();

        /// <summary>
        /// Add base ViewModel if it is required by the framework.
        /// </summary>
        /// <returns>SyntaxNodeOrToken with the ViewModel base.</returns>
        protected abstract SyntaxNodeOrToken AddBaseViewModel();

        /// <summary>
        /// Writes a message into the default logging system.
        /// </summary>
        /// <param name="message">The message to log.</param>
        [Conditional("DEBUG")]
        protected void LogMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Log?.LogMessage(message);
            }
        }

        /// <summary>
        /// Create the default namespaces.
        /// </summary>
        /// <param name="namespace">The namespace of the current class generated.</param>
        protected virtual void CreateNamespace(string @namespace)
        {
            this.NamespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(@namespace)).NormalizeWhitespace();

            // Add System using statement:
            var usings = new[] { "System", "System.Collections.Generic", "System.Threading", "System.Threading.Tasks" };

            foreach (var item in usings)
            {
                this.NamespaceDeclaration = this.NamespaceDeclaration.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(item)));
            }
        }

        /// <summary>
        /// Get command method syntax, depending on the framework used.
        /// </summary>
        /// <param name="command">The command to generate.</param>
        /// <returns>The ArgumentListSyntax that represent the command syntax to generate</returns>
        protected virtual ArgumentListSyntax GetCommandMethodSyntax(Command command)
        {
            var syntaxeNodeOrToken = new List<SyntaxNodeOrToken>();
            syntaxeNodeOrToken.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(command.FormatExecuteCommandName())));

            if (command.HasCanExecute)
            {
                syntaxeNodeOrToken.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                syntaxeNodeOrToken.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(command.FormatCanExecuteCommandName())));
            }

            return SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(syntaxeNodeOrToken));
        }

        /// <summary>
        /// Build base class to inherit.
        /// </summary>
        /// <param name="namespace">Namespace of the class.</param>
        /// <param name="class">The class name.</param>
        /// <returns>The SimpleBaseTypeSyntax of the base class.</returns>
        protected SimpleBaseTypeSyntax BuildBaseClass(string @namespace, string @class) =>
            SyntaxFactory.SimpleBaseType(SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName(@namespace), SyntaxFactory.IdentifierName(@class)));

        /// <summary>
        /// Creates the public properties for each Mvvm library.
        /// </summary>
        /// <returns>The public properties.</returns>
        /// <param name="property">The property to generate.</param>
        public virtual PropertyDeclarationSyntax CreatePublicProperties(Property property)
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
                                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName("Set")))
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
        /// Generate command comment to indicate which method associated to the command needs to be implemented.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The complete comment to the command.</returns>
        public virtual string GenerateCommandComment(Command command)
        {
            var defaultComment = $"// You must implement the following method(s): {command.FormatExecuteCommandName()}";
            var isComment = command.HasCanExecute ? $" and the {command.FormatCanExecuteCommandName()}" : string.Empty;
            return string.Concat(defaultComment, isComment);
        }

        private void GenerateViewModelImplementationFile(ViewModel viewModel)
        {
            this.CreateNamespace(viewModel.Namespace);
            this.CreateClass(viewModel);
            this.NamespaceDeclaration = this.NamespaceDeclaration.AddMembers(this.ClassDeclaration);

            // Normalize and get code as string.
            var content = this.NamespaceDeclaration.NormalizeWhitespace().ToFullString();

            FileHelper.SaveFileContent(this.Arguments.OutputFolderProject, viewModel.DestinationFolder, content, viewModel.CreateViewModelName(), ".cs", true);
        }

        private void GenerateViewModelPartialFile(ViewModel viewModel)
        {
            this.GenerateViewModel(viewModel);
            this.NamespaceDeclaration = this.NamespaceDeclaration.AddMembers(this.ClassDeclaration);
            this.NamespaceDeclaration = this.NamespaceDeclaration.AutoGeneratedCommand();

            // Normalize and get code as string.
            var content = this.NamespaceDeclaration.NormalizeWhitespace().ToFullString();

            FileHelper.SaveFileContent(this.Arguments.OutputFolderProject, viewModel.DestinationFolder, content, viewModel.CreateViewModelName(), ".part.g.cs", false);
        }

        private void GenerateViewModelInterfaceFile(ViewModel viewModel)
        {
            this.GenerateInterfaceViewModel(viewModel);
            this.NamespaceDeclaration = this.NamespaceDeclaration.AddMembers(this.InterfaceDeclaration);
            this.NamespaceDeclaration = this.NamespaceDeclaration.AutoGeneratedCommand();

            // Normalize and get code as string.
            var content = this.NamespaceDeclaration.NormalizeWhitespace().ToFullString();

            FileHelper.SaveFileContent(this.Arguments.OutputFolderProject, viewModel.DestinationFolder, content, viewModel.CreateViewModelName(), ".interface.g.cs", false);
        }

        private void GenerateViewModel(ViewModel viewModel)
        {
            this.CreateNamespace(viewModel.Namespace);
            this.CreateClass(viewModel);
            this.AddBaseClass(viewModel);

            var privateFields = new List<VariableDeclarationSyntax>();
            var publicProperties = new List<PropertyDeclarationSyntax>();

            // Create all properties.
            if (viewModel.Properties.Count > 0)
            {
                foreach (var property in viewModel.Properties)
                {
                    privateFields.Add(this.CreatePrivateVariable(property.Type.FindType(), property.Name));
                    publicProperties.Add(this.CreatePublicProperties(property));
                }
            }

            // Create all commands.
            if (viewModel.Commands.Count > 0)
            {
                foreach (var command in viewModel.Commands)
                {
                    var type = this.GetCommandType(command);
                    privateFields.Add(this.CreatePrivateVariable(type, command.FormatCommandName()));
                    publicProperties.Add(this.CreateCommandProperty(command));
                }
            }

            // Add all private fields to the class.
            foreach (var field in privateFields)
            {
                this.ClassDeclaration = this.ClassDeclaration.AddMembers(SyntaxFactory.FieldDeclaration(field)
                                                             .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)));
            }

            // Add all public properties to the class.
            foreach (var publicProperty in publicProperties)
            {
                this.ClassDeclaration = this.ClassDeclaration.AddMembers(publicProperty);
            }
        }

        private void GenerateInterfaceViewModel(ViewModel viewModel)
        {
            this.CreateNamespace(viewModel.Namespace);
            this.CreateInterface(viewModel);
            this.AddBaseInterface(viewModel);

            // Create and add all properties to the interface.
            foreach (var property in viewModel.Properties)
            {
                var field = this.CreatePublicProperty(property.Type.FindType(), property.Name)
                         .WithAccessorList(SyntaxFactory.AccessorList(
                            SyntaxFactory.SingletonList(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                         .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));

                this.InterfaceDeclaration = this.InterfaceDeclaration.AddMembers(field);
            }

            // Create and add all commands to the interface.
            foreach (var command in viewModel.Commands)
            {
                var field = this.CreatePublicProperty(typeof(System.Windows.Input.ICommand).ToString(), command.FormatCommandName())
                          .WithAccessorList(SyntaxFactory.AccessorList(
                             SyntaxFactory.SingletonList(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                          .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));

                this.InterfaceDeclaration = this.InterfaceDeclaration.AddMembers(field);
            }
        }

        private void CreateClass(ViewModel viewModel)
        {
            this.ClassDeclaration = SyntaxFactory.ClassDeclaration(viewModel.CreateViewModelName());
            this.ClassDeclaration = this.ClassDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                                         .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        }

        private void CreateInterface(ViewModel viewModel)
        {
            this.InterfaceDeclaration = SyntaxFactory.InterfaceDeclaration(string.Concat(viewModel.CreateInterfaceViewModelName()));
            this.InterfaceDeclaration = this.InterfaceDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        }

        private void AddBaseClass(ViewModel viewModel)
        {
            var baseClass = new List<SyntaxNodeOrToken>();

            if (viewModel.HasBase)
            {
                baseClass.Add(this.BuildBaseClass(viewModel.Namespace, viewModel.CreateBaseViewModelName()));
                baseClass.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
            }
            else
            {
                baseClass.Add(this.AddBaseViewModel());

                if (baseClass.Count > 0)
                {
                    baseClass.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                }
            }

            // Add the interface that will be associated with it
            baseClass.Add(this.BuildBaseClass(viewModel.Namespace, viewModel.CreateInterfaceViewModelName()));

            this.ClassDeclaration = this.ClassDeclaration.WithBaseList(this.CreateBaseClass(baseClass));
        }

        private void AddBaseInterface(ViewModel viewModel)
        {
            var baseClass = new List<SyntaxNodeOrToken>();

            if (viewModel.HasBase)
            {
                baseClass.Add(this.BuildBaseClass(viewModel.Namespace, viewModel.CreateInterfaceBaseViewModelName()));
            }
            else
            {
                baseClass.Add(this.BuildBaseClass("System.ComponentModel", "INotifyPropertyChanged"));
            }

            this.InterfaceDeclaration = this.InterfaceDeclaration.WithBaseList(this.CreateBaseClass(baseClass));
        }

        private PropertyDeclarationSyntax CreatePublicProperty(string type, string name) =>
            SyntaxFactory.PropertyDeclaration(SyntaxFactory.IdentifierName(type), SyntaxFactory.Identifier(name));

        private VariableDeclarationSyntax CreatePrivateVariable(string type, string name) =>
            SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(type)).AddVariables(SyntaxFactory.VariableDeclarator(name.ToCamelCase()));

        private BaseListSyntax CreateBaseClass(List<SyntaxNodeOrToken> baseClasses) =>
            SyntaxFactory.BaseList(SyntaxFactory.SeparatedList<BaseTypeSyntax>(baseClasses));

        private PropertyDeclarationSyntax CreateCommandProperty(Command command)
        {
            var name = command.FormatCommandName();
            var type = this.GetCommandType(command);

            return SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(typeof(System.Windows.Input.ICommand).ToString()), name)
              .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
              .AddAccessorListAccessors(
                      SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                      .WithExpressionBody(
                          SyntaxFactory.ArrowExpressionClause(
                              SyntaxFactory.BinaryExpression(
                                  SyntaxKind.CoalesceExpression,
                                  SyntaxFactory.MemberAccessExpression(
                                      SyntaxKind.SimpleMemberAccessExpression,
                                      SyntaxFactory.ThisExpression(),
                                      SyntaxFactory.IdentifierName(name.ToCamelCase())),
                                  SyntaxFactory.ParenthesizedExpression(
                                      SyntaxFactory.AssignmentExpression(
                                          SyntaxKind.SimpleAssignmentExpression,
                                          SyntaxFactory.MemberAccessExpression(
                                              SyntaxKind.SimpleMemberAccessExpression,
                                              SyntaxFactory.ThisExpression(),
                                              SyntaxFactory.IdentifierName(name.ToCamelCase())),
                                          SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName(type))
                                            .WithArgumentList(this.GetCommandMethodSyntax(command)))))))
              .WithSemicolonToken(SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(),
                            SyntaxKind.SemicolonToken,
                            SyntaxFactory.TriviaList(SyntaxFactory.Comment(this.GenerateCommandComment(command))))))
              .WithModifiers(FormatterHelper.GenerateComment(command.Comment));
        }

        private string GetCommandType(Command command)
        {
            var type = command.IsAsync ? this.GetAsyncCommandNamespace() : this.GetCommandNamespace();
            return command.HasParameterType ? $"{type}<{command.ParameterType}>" : type;
        }

        private void GenerateTarget(List<ViewModel> viewModels, out string generatedTargetFilename)
        {
            var xml = new XElement("Project");

            xml.Add(new XComment("This file has been generated with MvvmCodeGenerator, do not modify it."));

            var group = new XElement("ItemGroup");

            foreach (var viewModel in viewModels)
            {
                var name = viewModel.CreateViewModelName();
                var vmFolder = viewModel.DestinationFolder;

                var compile = new XElement("Compile");

                if (string.IsNullOrEmpty(vmFolder))
                {
                    compile.Add(new XAttribute("Update", $"{name}.*.g.cs"));
                }
                else
                {
                    compile.Add(new XAttribute("Update", string.Concat(Path.Combine(vmFolder, name), ".*.g.cs")));
                }

                var dependentUpon = new XElement("DependentUpon");
                dependentUpon.Add(new XText($"{name}.cs"));
                compile.Add(dependentUpon);

                group.Add(compile);
            }

            xml.Add(group);
            Console.WriteLine(xml);
            FileHelper.SaveFileContent(this.Arguments.OutputFolderProject, string.Empty, xml, GeneratedTargetFileWithoutExtension, GeneratedTargetFileExtension);
            generatedTargetFilename = string.Concat(GeneratedTargetFileWithoutExtension, GeneratedTargetFileExtension);
        }

        /// <summary>
        /// Inject the import of the generated project into the original project, if necessary.
        /// </summary>
        /// <param name="generatedTargetFilename">The name of the project to import.</param>
        private void InjectProject(string generatedTargetFilename)
        {
            LogMessage($@"Generated target filename: ""{generatedTargetFilename}""");
            if (!string.IsNullOrEmpty(generatedTargetFilename))
            {
                XmlDocument document = new XmlDocument();
                var projectPath = Arguments.ProjectPath;
                document.Load(projectPath);
                var node = document.SelectSingleNode($"/Project/Import[@Project='{generatedTargetFilename}']");

                LogMessage($"The generated project {(node == null ? "has not" : "has already")} been imported in the original project");
                if (node == null)
                {
                    node = document.CreateElement("Import");
                    var attribute = document.CreateAttribute("Project");
                    attribute.Value = generatedTargetFilename;
                    node.Attributes.Append(attribute);
                    document.SelectSingleNode("/Project").AppendChild(node);

                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        CheckCharacters = true,
                        CloseOutput = true,
                        Encoding = new UTF8Encoding(false),
                        Indent = true,
                        IndentChars = "  ",
                        NewLineChars = "\r\n",
                        NewLineHandling = NewLineHandling.None,
                        NewLineOnAttributes = false,
                        OmitXmlDeclaration = true
                    };
                    using (var stream = new FileStream(projectPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var xmlWriter = XmlWriter.Create(stream, settings);
                        using (xmlWriter)
                        {
                            LogMessage("Overwrite the original project");
                            document.WriteContentTo(xmlWriter);
                            xmlWriter.Flush();
                        }
                    }
                }
            }
        }
    }
}
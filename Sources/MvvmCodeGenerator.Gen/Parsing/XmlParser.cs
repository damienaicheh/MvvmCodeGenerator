namespace MvvmCodeGenerator.Gen
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// The Xml parser to get C# objects.
    /// </summary>
    public class XmlParser
    {
        public ResourceFile ReadResourceFile(string content)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(content);

            XmlNodeList generatorNode = xml.GetElementsByTagName("Generator");
            var generator = generatorNode[0].Attributes["Value"]?.Value;

            var viewModels = new List<ViewModel>();

            XmlNodeList allViewModelsTag = xml.GetElementsByTagName("ViewModels");

            foreach (XmlElement viewModelTag in allViewModelsTag)
            {
                var @namespace = viewModelTag.Attributes["Namespace"]?.Value;
                var destinationFolder = viewModelTag.Attributes["DestinationFolder"]?.Value ?? string.Empty;
                viewModels.AddRange(this.GetViewModels(viewModelTag, @namespace, destinationFolder));
            }


            return new ResourceFile
            {
                Generator = generator,
                ViewModels = viewModels
            };
        }

        /// <summary>
        /// Parse the xml to a list of ViewModels.
        /// </summary>
        /// <param name="xml">The xml resource file.</param>
        /// <param name="namespace">The destination namespace.</param>
        /// <param name="destinationFolder">The destination folder for the generated files.</param>
        /// <returns>The list of ViewModels parsed.</returns>
        private List<ViewModel> GetViewModels(XmlElement xml, string @namespace, string destinationFolder)
        {
            var viewModels = new List<ViewModel>();

            var viewModelsTag = xml.GetElementsByTagName("ViewModel");
            var itemViewModelsTag = xml.GetElementsByTagName("ItemViewModel");
            var allViewModelsTag = viewModelsTag.Cast<XmlNode>().Concat(itemViewModelsTag.Cast<XmlNode>()).ToList();

            foreach (XmlNode node in allViewModelsTag)
            {
                var isItemViewModel = node.Name == "ItemViewModel";
                var key = node.Attributes["Key"]?.Value;
                var baseViewModel = node.Attributes["Base"]?.Value;

                var viewModel = new ViewModel(isItemViewModel, key, baseViewModel, @namespace, destinationFolder);

                if (node.HasChildNodes)
                {
                    foreach (XmlNode child in node.ChildNodes.Cast<XmlNode>().Where(n => n.NodeType == XmlNodeType.Element))
                    {
                        var name = child.Attributes["Name"]?.Value;
                        var comment = child.Attributes["Description"]?.Value;

                        if (child.Name == "Property")
                        {
                            var parameterType = child.Attributes["Type"]?.Value;
                            viewModel.Properties.Add(new Property(name, parameterType, comment, true, true));
                        }
                        else if (child.Name.Contains("Command"))
                        {
                            var isAsync = child.Name.Split(new[] { "Command" }, System.StringSplitOptions.None)[0] == "Async";
                            var canExecute = child.Attributes["CanExecute"]?.Value.ToLower() == "true";
                            var parameterType = child.Attributes["Parameter"]?.Value;
                            viewModel.Commands.Add(new Command(isAsync, name, parameterType, comment, canExecute));
                        }
                    }
                }

                viewModels.Add(viewModel);
            }

            return viewModels;
        }
    }
}
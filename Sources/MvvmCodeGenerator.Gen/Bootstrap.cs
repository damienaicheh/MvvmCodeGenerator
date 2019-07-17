namespace MvvmCodeGenerator.Gen
{
    using System.IO;

    /// <summary>
    /// Bootstrap the generator. 
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// Start the generation.
        /// </summary>
        /// <param name="filePath">The path to the Resource file.</param>
        /// <param name="arguments">The arguments from the project.</param>
        public static void Start(string filePath, Arguments arguments)
        {
            var content = File.ReadAllText(filePath);

            XmlParser xmlParser = new XmlParser();
            ResourceFile resourceFile = xmlParser.ReadResourceFile(content);

            if (resourceFile.Generator != null)
            {               
                CSharpGenerator gen = null;

                switch (resourceFile.Generator.ToLower())
                {
                    case "mvvmicro":
                        gen = new MvvmicroCSharpGenerator(resourceFile.ViewModels, arguments);
                        break;
                    case "mvvmlightlibs":
                        gen = new MvvmLightLibsGenerator(resourceFile.ViewModels, arguments);
                        break;
                    case "mvvmcross":
                        gen = new MvvmCrossGenerator(resourceFile.ViewModels, arguments);
                        break;
                    case "freshmvvm":
                        gen = new FreshMvvmGenerator(resourceFile.ViewModels, arguments);
                        break;
                }

                gen.CleanGeneratedFiles();
                gen.Generate();
            }
        }
    }
}

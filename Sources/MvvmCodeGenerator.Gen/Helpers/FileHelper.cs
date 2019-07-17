namespace MvvmCodeGenerator.Gen
{
    using System;
    using System.IO;
    using System.Xml.Linq;

    /// <summary>
    /// File helper class to manage the generation files.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Clean the specified files dependening on their extensions and their folder path.
        /// </summary>
        /// <param name="folderPath">Folder path.</param>
        /// <param name="extensions">The file extensions.</param>
        public static void Clean(string folderPath, string extensions)
        {
            if (Directory.Exists(folderPath))
            {
                foreach (var file in Directory.GetFiles(folderPath))
                {
                    if (file.EndsWith(extensions, StringComparison.Ordinal))
                    {
                        Console.WriteLine($"Cleaning file: { file}");
                        File.Delete(file);
                    }
                }

                foreach (var file in Directory.GetDirectories(folderPath))
                {
                    Clean(file, extensions);
                }
            }
        }

        /// <summary>
        /// Saves the content of the file.
        /// </summary>
        /// <param name="outputFolder">Output folder.</param>
        /// <param name="destinationFolder">Destination folder.</param>
        /// <param name="content">The file content.</param>
        /// <param name="fileName">The File name.</param>
        /// <param name="fileExtension">The file extension.</param>
        public static void SaveFileContent(string outputFolder,string destinationFolder, string content, string fileName, string fileExtension, bool checkIfExist)
        {
            var folder = Path.Combine(outputFolder, destinationFolder);
            Console.WriteLine($"Folder destination {folder}");

            var path = Path.Combine(folder, string.Concat(fileName, fileExtension));

            if (checkIfExist)
            {
                if (!File.Exists(path))
                {
                    Save(path, content);
                }
            }
            else
            {
                Save(path, content);
            }

            // Output new interfaceCode to the build.
            Console.WriteLine(content);
        }

        /// <summary>
        /// Saves the content of the file.
        /// </summary>
        /// <param name="outputFolder">Output folder.</param>
        /// <param name="destinationFolder">Destination folder.</param>
        /// <param name="content">The xml file content.</param>
        /// <param name="fileName">The File name.</param>
        /// <param name="fileExtension">The file extension.</param>
        public static void SaveFileContent(string outputFolder, string destinationFolder, XElement content, string fileName, string fileExtension)
        {
            var folder = Path.Combine(outputFolder, destinationFolder);
            Console.WriteLine($"Folder destination {folder}");

            var path = Path.Combine(folder, string.Concat(fileName, fileExtension));
 
            using (var memory = new MemoryStream())
            {
                content.Save(memory);

                memory.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(memory))
                {
                    var text = reader.ReadToEnd();
                    Save(path, text);
                }
            }
        }

        /// <summary>
        /// Save the specified content into the file path.
        /// </summary>
        /// <param name="path">The path to the file to save data.</param>
        /// <param name="content">The content of the class.</param>
        private static void Save(string path, string content)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var folder = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(path, content);
            Console.WriteLine($"Save file {path}");
        }

    }
}

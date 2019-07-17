using System;
using System.IO;
using MvvmCodeGenerator.Gen;

namespace MvvmCodeGenerator.Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Args:");
            foreach (var arg in args)
            {
                Console.WriteLine($"  {arg}");
            }

            // Run this command to test the sample project:
            // dotnet run --project ./MvvmCodeGenerator.Gen/MvvmCodeGenerator.Gen.csproj -i:"./MvvmCodeGenerator.Sample/MvvmCodeGenMapper.xml" -o:"./MvvmCodeGenerator.Sample" -g:mvvmicro
            // var inputFile = args[0]?.Split("-i:")[1];
            // var outputFolderProject = args[1]?.Split("-o:")[1];
            // var content = File.ReadAllText(inputFile);
            // Run the project directly with this configuration:

            var outputFolderProject = "./MvvmCodeGenerator.Dev";
            
            Arguments arguments = new Arguments
            {
                OutputFolderProject = outputFolderProject
            };

            Bootstrap.Start("./../../../../MvvmCodeGenerator.Dev/MvvmCodeGenMapper.xml", arguments);

            Console.WriteLine("End of generation.");
        }
    }
}

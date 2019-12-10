using System;
using System.IO;
using AocIntComputer.Compiler;

namespace Compiler {
    static class Program {
        private static string FileNameWithoutExtension(this FileInfo file) {
            return file.FullName.Substring(
                0,
                file.FullName.LastIndexOf(".", StringComparison.Ordinal)
            );
        }

        public static void Main(string[] args) {
            FileInfo fileIn = new FileInfo(args[0]);

            if (!fileIn.Exists) {
                Console.WriteLine($"File \"{args[0]}\" does not exist");
            }

            FileInfo fileOut = new FileInfo(fileIn.FileNameWithoutExtension() + ".INTCOMPUTER");

            long[] compiledProgram = new IntComputerCompiler(fileIn.OpenText().ReadToEnd()).Compile();

            using (StreamWriter fout = fileOut.CreateText()) {
                fout.Write(string.Join(",", compiledProgram));
            }

            Console.WriteLine($"Program {fileIn.Name} compiled to file {fileOut.Name}");
        }
    }
}
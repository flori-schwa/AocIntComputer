using System;
using System.IO;
using AocIntComputer.Compiler;

namespace Compiler {
    class Program {
        public static void Main(string[] args) {
            /*
             * Options:
             *     --binary - to binary file
             */

            FileInfo fileIn = new FileInfo(args[0]);

            if (!fileIn.Exists) {
                Console.WriteLine($"File \"{args[0]}\" does not exist");
            }

            FileInfo fileOut = new FileInfo(fileIn.Name + ".INT");

            long[] compiledProgram = new IntComputerCompiler(fileIn.OpenText().ReadToEnd()).Compile();

            using (StreamWriter fout = fileOut.CreateText()) {
                fout.Write(string.Join(",", compiledProgram));
            }

            Console.WriteLine($"Program {fileIn.Name} compiled to file {fileOut.Name}");
        }
    }
}
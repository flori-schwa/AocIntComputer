using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AocIntComputer.Runtime;

namespace Runtime {
    class Flags {
        public Flags(params string[] args) {
            IEnumerable<PropertyInfo> properties = this.GetType().GetProperties();

            foreach (string flag in args) {
                if (!flag.StartsWith("--")) {
                    continue;
                }

                string name = flag.Substring(2);

                foreach (PropertyInfo propertyInfo in properties) {
                    if (propertyInfo.Name.ToLower().Equals(name.ToLower())) {
                        propertyInfo.SetValue(this, true);
                    }
                }
            }
        }

        public bool Debug { get; set; } = false;
    }

    static class Program {
        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Syntax: <executable file>");
                return;
            }

            FileInfo fileInfo = new FileInfo(args[0]);
            Flags flags = new Flags(args);

            if (!fileInfo.Exists) {
                Console.WriteLine($"File \"{args[0]}\" does not exist!");
                return;
            }

            long[] program;

            using (StreamReader reader = fileInfo.OpenText()) {
                program = reader.ReadToEnd().Split(',').Select(long.Parse).ToArray();
            }

            new SimpleIntComputer(program, flags.Debug).Run();
        }
    }
}
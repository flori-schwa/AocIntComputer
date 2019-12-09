using System;
using System.Collections.Generic;

namespace AocIntComputer.Runtime {
    public class SimpleIntComputer : BaseIntComputer {
        public SimpleIntComputer(IEnumerable<long> program, bool debug = false) : base(program, debug) { }

        protected override long Input() {
            Console.Write("> ");
            return long.Parse(Console.ReadLine());
        }

        protected override void Out(long x) {
            Console.WriteLine(x);
        }
    }
}
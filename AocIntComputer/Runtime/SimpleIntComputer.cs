using System;
using System.Collections.Generic;

namespace AocIntComputer.Runtime {
    public class SimpleIntComputer : BaseIntComputer {
        public SimpleIntComputer(List<long> program) : base(program) { }

        protected override long Input() {
            Console.WriteLine("IN: ");
            return long.Parse(Console.ReadLine());
        }

        protected override void Out(long x) {
            Console.WriteLine(x);
        }
    }
}
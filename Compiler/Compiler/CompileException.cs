using System;
using System.Runtime.Serialization;

namespace AocIntComputer.Compiler {
    public class CompileException : Exception {
        public CompileException() { }
        protected CompileException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public CompileException(string message) : base(message) { }
        public CompileException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InvalidArgumentCountException : CompileException {
        public InvalidArgumentCountException(int expected, int actual) : base(
            $"Invalid argument count received, expected {expected}, got {actual}") { }
    }
}
using AocIntComputer.Runtime;

namespace AocIntComputer.Compiler.Tokens {
    public class TokenEnd : InstructionToken {
        public override string TokenValue => "END";

        public override int RequiredParameters => 0;

        public override int SizeLongs => 1;

        public override long[] Compile(params ParameterToken[] parameters) {
            if (parameters.Length != RequiredParameters) {
                throw new InvalidArgumentCountException(RequiredParameters, parameters.Length);
            }

            long[] compiled = new long[SizeLongs];
            compiled[0] = BaseIntComputer.OpCodeEnd;

            return compiled;
        }
    }
}
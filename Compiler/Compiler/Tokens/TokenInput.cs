using AocIntComputer.Runtime;

namespace AocIntComputer.Compiler.Tokens {
    public class TokenInput : InstructionToken {
        public override string TokenValue => "IN";

        public override int RequiredParameters => 1;

        public override int SizeLongs => 2;

        public override long[] Compile(params ParameterToken[] parameters) {
            if (parameters.Length != RequiredParameters) {
                throw new InvalidArgumentCountException(RequiredParameters, parameters.Length);
            }
            
            long[] compiled = new long[SizeLongs];
            
            compiled[0] = BaseIntComputer.OpCodeInput + EncodeParameterModes(parameters);
            compiled[1] = RequireNotImmediateMode(0, parameters);

            return compiled;
        }
    }
}
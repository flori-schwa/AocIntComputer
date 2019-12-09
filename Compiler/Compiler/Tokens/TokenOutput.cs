using AocIntComputer.Runtime;

namespace AocIntComputer.Compiler.Tokens {
    public class TokenOutput : InstructionToken {
        public override string TokenValue => "OUT";

        public override int RequiredParameters => 1;

        public override int SizeLongs => 2;

        public override long[] Compile(params ParameterToken[] parameters) {
            if (parameters.Length != RequiredParameters) {
                throw new InvalidArgumentCountException(RequiredParameters, parameters.Length);
            }
            
            long[] compiled = new long[SizeLongs];
            
            compiled[0] = BaseIntComputer.OpCodeOutput + EncodeParameterModes(parameters);
            compiled[1] = parameters[0].Value.Value;

            return compiled;
        }
    }
}
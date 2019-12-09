using AocIntComputer.Runtime;

namespace AocIntComputer.Compiler.Tokens {
    public class TokenAdjustRelativeBase : InstructionToken {
        public override string TokenValue => "ADJR";

        public override int RequiredParameters => 1;

        public override int SizeLongs => 2;
        
        public override long[] Compile(params ParameterToken[] parameters) {
            if (parameters.Length != RequiredParameters) {
                throw new InvalidArgumentCountException(RequiredParameters, parameters.Length);
            }
            
            long[] compiled = new long[SizeLongs];

            compiled[0] = BaseIntComputer.OpCodeAdjustRelativeBase + EncodeParameterModes(parameters);
            compiled[1] = parameters[0].Value.Value;

            return compiled;
        }
    }
}
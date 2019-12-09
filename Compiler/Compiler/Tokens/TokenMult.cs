using AocIntComputer.Runtime;

namespace AocIntComputer.Compiler.Tokens {
    public class TokenMult : InstructionToken {
        public override string TokenValue => "MULT";

        public override int RequiredParameters => 3;
        
        public override int SizeLongs => 4;

        public override long[] Compile(params ParameterToken[] parameters) {
            if (parameters.Length != RequiredParameters) {
                throw new InvalidArgumentCountException(RequiredParameters, parameters.Length);
            }

            long[] compiled = new long[SizeLongs];
            
            compiled[0] = BaseIntComputer.OpCodeMult + EncodeParameterModes(parameters);;
            compiled[1] = parameters[0].Value.Value;
            compiled[2] = parameters[1].Value.Value;
            compiled[3] = RequireNotImmediateMode(2, parameters);

            return compiled;
        }
    }
}
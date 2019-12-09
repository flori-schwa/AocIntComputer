using AocIntComputer.Runtime;

namespace AocIntComputer.Compiler.Tokens {
    public class TokenAdd : InstructionToken {
        public override string TokenValue => "ADD";

        public override int RequiredParameters => 3;

        public override int SizeLongs => 4;

        public override long[] Compile(params ParameterToken[] parameters) {
            if (parameters.Length != RequiredParameters) {
                throw new InvalidArgumentCountException(RequiredParameters, parameters.Length);
            }

            long[] compiled = new long[SizeLongs];
            
            compiled[0] = BaseIntComputer.OpCodeAdd + EncodeParameterModes(parameters);;
            compiled[1] = parameters[0].Value.Value;
            compiled[2] = parameters[1].Value.Value;
            compiled[3] = RequireNotImmediateMode(2, parameters);

            return compiled;
        }
    }
}
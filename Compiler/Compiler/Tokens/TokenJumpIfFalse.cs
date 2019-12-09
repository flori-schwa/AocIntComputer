using AocIntComputer.Runtime;

namespace AocIntComputer.Compiler.Tokens {
    public class TokenJumpIfFalse : InstructionToken {
        public override string TokenValue => "JMPF";

        public override int RequiredParameters => 2;

        public override long[] Compile(params ParameterToken[] parameters) {
            if (parameters.Length != RequiredParameters) {
                throw new InvalidArgumentCountException(RequiredParameters, parameters.Length);
            }
            
            long[] compiled = new long[SizeLongs];

            compiled[0] = BaseIntComputer.OpCodeJumpIfFalse + EncodeParameterModes(parameters);
            compiled[1] = parameters[0].Value.Value;
            compiled[2] = parameters[1].Value.Value;

            return compiled;
        }

        public override int SizeLongs => 3;
    }
}
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using AocIntComputer.Compiler.Tokens;
using AocIntComputer.Runtime;

namespace AocIntComputer.Compiler {
    public enum TokenType {
        Instruction,
        Parameter
    }

    public abstract class BaseToken {
        public static readonly InstructionToken TokenAdd = new TokenAdd();
        public static readonly InstructionToken TokenMultiply = new TokenMult();
        public static readonly InstructionToken TokenInput = new TokenInput();
        public static readonly InstructionToken TokenOutput = new TokenOutput();
        public static readonly InstructionToken TokenJumpIfTrue = new TokenJumpIfTrue();
        public static readonly InstructionToken TokenJumpIfFalse = new TokenJumpIfTrue();
        public static readonly InstructionToken TokenLessThan = new TokenLessThan();
        public static readonly InstructionToken TokenEquals = new TokenEquals();
        public static readonly InstructionToken TokenAdjustRelativeBase = new TokenAdjustRelativeBase();
        public static readonly InstructionToken TokenEnd = new TokenEnd();

        public static readonly InstructionToken[] Instructions = {
            TokenAdd,
            TokenMultiply,
            TokenInput,
            TokenOutput,
            TokenJumpIfTrue,
            TokenJumpIfFalse,
            TokenLessThan,
            TokenEquals,
            TokenAdjustRelativeBase,
            TokenEnd
        };

        public abstract TokenType TokenType { get; }
    }

    public abstract class InstructionToken : BaseToken {
        public sealed override TokenType TokenType => TokenType.Instruction;

        public abstract string TokenValue { get; }
        
        public abstract int RequiredParameters { get; }

        public abstract int SizeLongs { get; }

        public int SizeBytes => SizeLongs * sizeof(long);
        
        public abstract long[] Compile(params ParameterToken[] parameters);

        protected static long RequireNotImmediateMode(int index, params ParameterToken[] parameters) {
            if (parameters[index].Value.ParameterMode == ParameterMode.Immediate) {
                throw new CompileException("Parameter mode of write to address may not be in immediate mode!");
            }

            return parameters[index].Value.Value;
        }

        protected long EncodeParameterModes(params ParameterToken[] parameters) {
            long modes = 0;

            for (int i = 0; i < RequiredParameters; i++) {
                switch (parameters[i].Value.ParameterMode) {
                    case ParameterMode.Position: {
                        break;
                    }

                    case ParameterMode.Immediate: {
                        modes += (long) Math.Pow(10, (RequiredParameters + 1) - i);
                        break;
                    }

                    case ParameterMode.Relative: {
                        modes += 2 * (long) Math.Pow(10, (RequiredParameters + 1) - i);
                        break;
                    }

                    default: {
                        throw new CompileException("Unknown parameter mode");
                    }
                }
            }

            return modes;
        }
    }

    public class ParameterToken : BaseToken {
        public sealed override TokenType TokenType => TokenType.Parameter;

        public Parameter Value { get; }

        public ParameterToken(Parameter value) {
            Value = value;
        }
    }

    public class Parameter {

        public static Parameter Parse(string parameter) {
            /*
             * Hex prefix '0x'
             *
             * position mode: no prefix
             * immediate mode: $
             * relative mode: R
             * 
             */

            char prefix = parameter[0];

            ParameterMode mode;

            switch (prefix) {
                case '$': {
                    mode = ParameterMode.Immediate;
                    break;
                }

                case 'R': {
                    mode = ParameterMode.Relative;
                    break;
                }

                default: {
                    if (prefix < '0' || prefix > '9') {
                        throw new CompileException($"Cannot parse parameter {parameter}");
                    }

                    mode = ParameterMode.Position;
                    break;
                }
            }

            if (mode != ParameterMode.Position) {
                parameter = parameter.Substring(1); // Remove prefix
            }

            long value = Convert.ToInt64(parameter);

            return new Parameter(value, mode);
        }

        public Parameter(long value, ParameterMode parameterMode) {
            Value = value;
            ParameterMode = parameterMode;
        }

        public long Value { get; }

        public ParameterMode ParameterMode { get; }
    }

    public class Statement {
        private InstructionToken _instruction;
        private ParameterToken[] _parameters;

        public static Statement Parse(string line) {
            string[] parts = line.Trim().Split(' ');

            InstructionToken instructionToken =
                BaseToken.Instructions.FirstOrDefault(t => t.TokenValue.Equals(parts[0]));

            if (instructionToken == null) {
                throw new CompileException($"Unknown instruction \"{parts[0]}\"");
            }
            
            ParameterToken[] parameterTokens = new ParameterToken[parts.Length - 1];

            for (int i = 1; i < parts.Length; i++) {
                parameterTokens[i - 1] = new ParameterToken(Parameter.Parse(parts[i]));
            }
            
            return new Statement(instructionToken, parameterTokens);
        }

        private Statement(InstructionToken instruction, ParameterToken[] parameters) {
            _instruction = instruction;
            _parameters = parameters;
        }

        public InstructionToken Instruction => _instruction;

        public ParameterToken[] Parameters => _parameters;

        public long[] Compile() {
            return Instruction.Compile(Parameters);
        }
    }

    public class IntComputerCompiler {
        private string _program;

        public IntComputerCompiler(string program) {
            _program = program;
        }

        public long[] Compile() {
            string[] lines = _program.Split('\n');
            
            LongWriter longWriter = new LongWriter();

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();
                line = line.Substring(0, line.Length - line.IndexOf('#')).Trim();

                if (line.Length == 0) {
                    continue;
                }
                
                longWriter.Write(Statement.Parse(line).Compile());
            }

            return longWriter.ToArray();
        }
    }
}
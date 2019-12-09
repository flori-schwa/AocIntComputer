using System;
using System.Collections.Generic;
using System.Linq;
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

            for (int i = 0; i < parameters.Length; i++) {
                switch (parameters[i].Value.ParameterMode) {
                    case ParameterMode.Position: {
                        break;
                    }

                    case ParameterMode.Immediate: {
                        modes += (long) Math.Pow(10, RequiredParameters + 1 - (parameters.Length - 1 - i));
                        break;
                    }

                    case ParameterMode.Relative: {
                        modes += 2 * (long) Math.Pow(10, RequiredParameters + 1 - (parameters.Length - 1 - i));
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

        public override string ToString() => Value.ToString();
    }

    public class Parameter {
        public static Parameter Parse(string parameter, IDictionary<string, long> variables) {
            /*
             * Hex prefix '0x'
             *
             * position mode: no prefix
             * immediate mode: $
             * relative mode: R
             * 
             */

            if (variables.ContainsKey(parameter)) {
                return new Parameter(variables[parameter], ParameterMode.Position);
            }

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

            return new Parameter(parameter.ParseLongDecimalOrHex(), mode);
        }

        public override string ToString() {
            string s = "";

            switch (ParameterMode) {
                case ParameterMode.Immediate: {
                    s += "$";
                    break;
                }

                case ParameterMode.Relative: {
                    s += "R";
                    break;
                }
            }

            return s + Value;
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

        private static bool IsValidVariableName(string name) {
            return name.All(c => c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z');
        }

        public static Statement Parse(string line, long index, IDictionary<string, long> variables) {
            string[] parts = line.Trim().Split(' ');

            InstructionToken instructionToken =
                BaseToken.Instructions.FirstOrDefault(t => t.TokenValue.Equals(parts[0]));
            ParameterToken[] parameterTokens;

            if (instructionToken == null) {
                // Data line or variable declaration
                
                // Variable declaration:
                // var name := value

                if (parts.Length == 3 && "var".Equals(parts[0])) {
                    string varName = parts[1];

                    if (variables.ContainsKey(varName)) {
                        throw new CompileException($"A variable with the name \"{varName}\" already exists!");
                    }

                    if (!IsValidVariableName(varName)) {
                        throw new CompileException($"Illegal variable name \"{varName}\"");
                    }

                    variables[varName] = index; // Store the variable index

                    parts = new[] { parts[2].ParseLongDecimalOrHex().ToString() }; // Write the variable value
                }

                parameterTokens = new ParameterToken[parts.Length];

                for (int i = 0; i < parts.Length; i++) {
                    parameterTokens[i] =
                        new ParameterToken(new Parameter(parts[i].ParseLongDecimalOrHex(), ParameterMode.Position));
                }
            }
            else {
                parameterTokens = new ParameterToken[parts.Length - 1];

                for (int i = 1; i < parts.Length; i++) {
                    parameterTokens[i - 1] = new ParameterToken(Parameter.Parse(parts[i], variables));
                }
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
            if (Instruction != null) {
                return Instruction.Compile(Parameters);
            }

            return Parameters.Select(p => p.Value.Value).ToArray();
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
            IDictionary<string, long> variables = new Dictionary<string, long>();

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();

                int hashIndex = line.IndexOf('#');

                if (hashIndex >= 0) {
                    line = line.Substring(0, hashIndex).Trim();
                }

                if (line.Length == 0) {
                    continue;
                }

                longWriter.Write(Statement.Parse(line, longWriter.Length, variables).Compile());
            }

            return longWriter.ToArray();
        }
    }
}
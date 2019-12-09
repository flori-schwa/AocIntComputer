using System;
using System.Collections.Generic;
using System.Linq;

namespace AocIntComputer.Runtime {
    
    public enum ParameterMode : long {
        Position,
        Immediate,
        Relative
    }
    
    public abstract class BaseIntComputer {
        
        class IntComputerMemory {
            private long[] _mem;

            public IntComputerMemory(long[] mem) {
                _mem = mem;
            }

            public long this[long index] {
                get {
                    if (index >= _mem.Length) {
                        Grow(index + 1);
                    }

                    return _mem[index];
                }

                set {
                    if (index >= _mem.Length) {
                        Grow(index + 1);
                    }

                    _mem[index] = value;
                }
            }

            public int Length => _mem.Length;

            private void Grow(long length) {
                long[] newMem = new long[length];

                Array.Copy(_mem, newMem, Length);
                _mem = newMem;
            }

            public long[] ToArray => _mem;
        }

        public const long OpCodeAdd = 1;
        public const long OpCodeMult = 2;
        public const long OpCodeInput = 3;
        public const long OpCodeOutput = 4;
        public const long OpCodeJumpIfTrue = 5;
        public const long OpCodeJumpIfFalse = 6;
        public const long OpCodeLessThan = 7;
        public const long OpCodeEquals = 8;
        public const long OpCodeAdjustRelativeBase = 9;
        public const long OpCodeEnd = 99;

        private IntComputerMemory _mem;

        private bool _halt, _finished, _debug;
        private long _pc = 0, _relativeBase = 0;

        public BaseIntComputer(IEnumerable<long> program, bool debug = false) {
            _mem = new IntComputerMemory(program.ToArray());
            _debug = debug;
        }

        public bool IsFinished => _finished;

        public bool IsHalted => _halt;

        public void Halt() => _halt = true;

        protected long PeekOpCode() => _mem[_pc];

        public long[] Resume() {
            _halt = false;
            return Run();
        }

        public long[] Run() {
            if (_finished) {
                return _mem.ToArray;
            }

            while (true) {
                if (_halt) {
                    goto end;
                }

                if (_debug) {
                    Console.WriteLine($"[{string.Join(",", _mem.ToArray)}]");
                    Console.WriteLine($"PC {_pc}");
                    Console.WriteLine($"NextOp {PeekOpCode()}");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }

                ParameterMode[] modes = {ParameterMode.Position, ParameterMode.Position, ParameterMode.Position};
                long opCode = _mem[_pc++];
                int modeIndex = 0;

                if (opCode >= 100) {
                    if (opCode / 10_000 != 0) {
                        long mode = opCode / 10_000;

                        modes[2] = mode == (long) ParameterMode.Immediate ? ParameterMode.Immediate : ParameterMode.Relative;
                        opCode -= mode * 10_000;
                    }

                    if (opCode / 1000 != 0) {
                        long mode = opCode / 1000;

                        modes[1] = mode == (long) ParameterMode.Immediate ? ParameterMode.Immediate : ParameterMode.Relative;
                        opCode -= mode * 1000;
                    }

                    if (opCode / 100 != 0) {
                        long mode = opCode / 100;

                        modes[0] = mode == (long) ParameterMode.Immediate ? ParameterMode.Immediate : ParameterMode.Relative;
                        opCode -= mode * 100;
                    }
                }

                long NextAddrWriteTo() {
                    switch (modes[modeIndex++]) {
                        case ParameterMode.Position: {
                            return _mem[_pc++];
                        }

                        case ParameterMode.Relative: {
                            return _relativeBase + _mem[_pc++];
                        }

                        default: {
                            throw new Exception();
                        }
                    }
                }

                long NextParameter() => GetParameter(_pc++, modes[modeIndex++]);

                switch (opCode) {
                    case OpCodeAdd: {
                        long a = NextParameter();
                        long b = NextParameter();

                        long storeTo = NextAddrWriteTo();

                        if (_debug) {
                            Console.WriteLine($"WRITE ({a} + {b}) => {a + b} TO ADDR {storeTo}");
                        }

                        _mem[storeTo] = a + b;
                        continue;
                    }

                    case OpCodeMult: {
                        long a = NextParameter();
                        long b = NextParameter();

                        long storeTo = NextAddrWriteTo();

                        if (_debug) {
                            Console.WriteLine($"WRITE ({a} * {b}) => {a * b} TO ADDR {storeTo}");
                        }

                        _mem[storeTo] = a * b;
                        continue;
                    }

                    case OpCodeInput: {
                        long storeTo = NextAddrWriteTo();
                        long input = Input();

                        if (_debug) {
                            Console.WriteLine($"WRITE INPUT \"{input}\" TO ADDR {storeTo}");
                        }

                        _mem[storeTo] = input;
                        continue;
                    }

                    case OpCodeOutput: {
                        switch (modes[0]) {
                            case ParameterMode.Position: {
                                Out(_mem[_mem[_pc++]]);
                                break;
                            }

                            case ParameterMode.Immediate: {
                                Out(_mem[_pc++]);
                                break;
                            }

                            case ParameterMode.Relative: {
                                Out(_mem[_relativeBase + _mem[_pc++]]);
                                break;
                            }
                        }
                        
                        continue;
                    }

                    case OpCodeJumpIfTrue: {
                        long check = NextParameter();
                        long jumpTo = NextParameter();

                        if (check != 0) {

                            if (_debug) {
                                Console.WriteLine($"JUMP TO ADDR {jumpTo} BECAUSE {check} IS TRUE");
                            }
                            
                            _pc = jumpTo;
                        }

                        continue;
                    }

                    case OpCodeJumpIfFalse: {
                        long check = NextParameter();
                        long jumpTo = NextParameter();

                        if (check == 0) {

                            if (_debug) {
                                Console.WriteLine($"JUMP TO ADDR {jumpTo} BECAUSE {check} IS FALSE");
                            }
                            
                            _pc = jumpTo;
                        }

                        continue;
                    }

                    case OpCodeLessThan: {
                        long a = NextParameter();
                        long b = NextParameter();
                        long writeTo = NextAddrWriteTo();

                        if (_debug) {
                            Console.WriteLine($"WRITE ({a} < {b}) => {(a < b ? 1 : 0)} TO ADDR {writeTo}");
                        }
                        
                        if (a < b) {
                            _mem[writeTo] = 1;
                        }
                        else {
                            _mem[writeTo] = 0;
                        }

                        continue;
                    }

                    case OpCodeEquals: {
                        long a = NextParameter();
                        long b = NextParameter();
                        long writeTo = NextAddrWriteTo();

                        if (_debug) {
                            Console.WriteLine($"WRITE ({a} == {b}) => {(a == b ? 1 : 0)} TO ADDR {writeTo}");
                        }

                        if (a == b) {
                            _mem[writeTo] = 1;
                        }
                        else {
                            _mem[writeTo] = 0;
                        }

                        continue;
                    }

                    case OpCodeAdjustRelativeBase: {
                        long modBase = NextParameter();

                        _relativeBase += modBase;
                        continue;
                    }

                    case OpCodeEnd: {
                        _finished = true;
                        goto end;
                    }

                    default: {
                        Console.WriteLine("Unknown opCode " + opCode);
                        goto end;
                    }
                }
            }

            end: ;

            return _mem.ToArray;
        }

        private long GetParameter(long programCounter, ParameterMode parameterMode) {
            switch (parameterMode) {
                case ParameterMode.Position: {
                    long addr = _mem[programCounter];
                    return _mem[addr];
                }

                case ParameterMode.Immediate: {
                    return _mem[programCounter];
                }

                case ParameterMode.Relative: {
                    long off = _mem[programCounter];
                    return _mem[_relativeBase + off];
                }

                default: {
                    throw new Exception();
                }
            }
        }

        protected abstract long Input();

        protected abstract void Out(long x);
    }
}
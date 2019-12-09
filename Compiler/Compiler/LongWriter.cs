using System;

namespace AocIntComputer.Compiler {
    public class LongWriter {
        private long[] _longs;
        private int _nextPosition = 0;

        public LongWriter(int initialCapacity = 0) {
            _longs = new long[initialCapacity];
        }

        public int Length => _longs.Length;

        private void Grow(int requiredSize) {
            if (requiredSize <= Length) {
                throw new ArgumentException();
            }

            long[] newLongs = new long[requiredSize];
            Array.Copy(_longs, newLongs, Length);

            _longs = newLongs;
        }

        public void EnsureCapacity(int length) {
            if (_nextPosition + length >= Length) {
                Grow(_nextPosition + length);
            }
        }

        public void Write(long l) {
            EnsureCapacity(1);
            _longs[_nextPosition++] = l;
        }

        public void Write(params long[] longs) {
            EnsureCapacity(longs.Length);

            foreach (long l in longs) {
                _longs[_nextPosition++] = l;
            }
        }

        public long[] ToArray() {
            return _longs;
        }
    }
}
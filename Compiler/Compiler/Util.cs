using System;

namespace AocIntComputer.Compiler {
    public static class Util {
        public static long ParseLongDecimalOrHex(this string s) {
            if (s.StartsWith("0x")) {
                return Convert.ToInt64(s, 16);
            }

            return Convert.ToInt64(s);
        }
    }
}
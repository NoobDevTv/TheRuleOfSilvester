using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester
{
    public static class ValueConverter
    {
        public static object Parse(string value)
        {     
            if (byte.TryParse(value, out byte parsedByte))
                return parsedByte;

            if (short.TryParse(value, out short parsedShort))
                return parsedShort;

            if (ushort.TryParse(value, out ushort parsedUshort))
                return parsedUshort;

            if (int.TryParse(value, out int parsedInt))
                return parsedInt;

            if (uint.TryParse(value, out uint parsedUint))
                return parsedUint;

            if (long.TryParse(value, out long parsedLong))
                return parsedLong;

            if (ulong.TryParse(value, out ulong parsedUlong))
                return parsedUlong;

            if (float.TryParse(value, out float parsedFloat))
                return parsedFloat;

            if (double.TryParse(value, out double parsedDouble))
                return parsedDouble;

            if (decimal.TryParse(value, out decimal parsedDecimal))
                return parsedDecimal;

            if (bool.TryParse(value, out bool parsedBool))
                return parsedBool;

            if (char.TryParse(value, out char parsedChar))
                return parsedChar;

            return value;
        }
    }
}

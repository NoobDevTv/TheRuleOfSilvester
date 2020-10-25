using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.UI.Drawing
{
    public readonly struct Color
    {
        public readonly byte A { get; }
        public readonly byte R { get; }
        public readonly byte G { get; }
        public readonly byte B { get; }

        public Color(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
        public Color(int colorValue)
        {
            B = (byte)colorValue;
            G = (byte)(colorValue << 8);
            R = (byte)(colorValue << 16);
            A = (byte)(colorValue << 24);
        }

        public int ToInt32()
            => (A << 24) | (R << 16) | (G << 8) | B;
    }
}

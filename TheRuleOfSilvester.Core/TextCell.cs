using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public class TextCell : Cell
    {
        public TextCell(string text) : base(text.Length, 1, null)
        {
            for (int i = 0; i < text.Length; i++)
                Lines[i, 0] = text[i];
        }
    }
}

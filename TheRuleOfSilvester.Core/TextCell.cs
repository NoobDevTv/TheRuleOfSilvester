using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public class TextCell : Cell
    {
        private string text;
        public TextCell(string text, Map map) : base(text.Length, 1, map)
        {
            this.text = text;
            for (int i = 0; i < text.Length; i++)
                Lines[i, 0] = text[i];
        }

        public void MakeBlank()
        {
            for (int i = 0; i < text.Length; i++)
                Lines[i, 0] = ' ';
            Invalid = true;            
        }

        public void MakeNotBlank()
        {
            for (int i = 0; i < text.Length; i++)
                Lines[i, 0] = text[i];
            Invalid = false;
        }
    }
}

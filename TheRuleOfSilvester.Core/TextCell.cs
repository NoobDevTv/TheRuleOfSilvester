using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public class TextCell : Cell
    {
        public string Text
        {
            get => text; set
            {
                if (value.Length != text?.Length)
                    Lines = new Cells.BaseElement[value.Length, 1];
                text = value;
                for (int i = 0; i < text.Length; i++)
                    Lines[i, 0] = text[i];
                Invalid = true;
            }
        }

        private string text;
        
        public TextCell(string text, int length, Map map) : base(length, 1, map)
        {
            Text = text;
        }
        public TextCell(string text, Map map) : this(text, text.Length, map)
        {
        }
        public TextCell(string text, Point pos, Map map) : this(text, map)
        {
            Position = pos;
        }

        public void MakeBlank()
        {
            for (int i = 0; i < Text.Length; i++)
                Lines[i, 0] = ' ';
            Invalid = true;
        }

        public void MakeNotBlank()
        {
            for (int i = 0; i < Text.Length; i++)
                Lines[i, 0] = Text[i];
            Invalid = false;
        }
    }
}

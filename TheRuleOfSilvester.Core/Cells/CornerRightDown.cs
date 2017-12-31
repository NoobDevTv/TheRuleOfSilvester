using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerRightDown : Cell
    {
        public CornerRightDown() : base()
        {
            Lines[2, 0] = "│";
            Lines[2, 4] = "┌";
            Lines[1, 0] = "│";
            Lines[0, 0] = "┌";
            for (int i = 1; i < 5; i++)
                Lines[0, i] = "─";
        }
    }
}

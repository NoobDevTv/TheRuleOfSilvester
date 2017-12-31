using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerLeftDown : Cell
    {
        public CornerLeftDown()
        {
            Lines[2, 4] = "│";
            Lines[2, 0] = "┐";
            Lines[1, 4] = "│";
            Lines[0, 4] = "┐";
            for (int i = 0; i < 4; i++)
                Lines[0, i] = "─";
        }
    }
}

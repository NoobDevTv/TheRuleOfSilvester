using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerRightUp : Cell
    {
        public CornerRightUp() : base()
        {

            Lines[0, 0] = "│";
            Lines[0, 4] = "└";
            Lines[1, 0] = "│";
            Lines[2, 0] = "└";
            for (int i = 1; i < 5; i++)
                Lines[2, i] = "─"; 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    class UpDownRight : Cell
    {
        public UpDownRight()
        {
            Lines[2, 4] = "┌";
            Lines[0, 4] = "└";
            for (int i = 0; i < 3; i++)
                Lines[i, 0] = "│";
        }
    }
}

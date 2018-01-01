using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    class UpDownLeft : Cell
    {
        public UpDownLeft(Map map) : base(map)
        {
            Lines[0, 0] = "┘";
            Lines[2, 0] = "┐";
            for (int i = 0; i < 3; i++)
                Lines[i, 4] = "│";
        }
    }
}

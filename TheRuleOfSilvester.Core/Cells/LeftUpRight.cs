using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    class LeftUpRight : Cell
    {
        public LeftUpRight(Map map) : base(map)
        {
            Lines[0, 0] = "┘";
            Lines[0, 4] = "└";
            for (int i = 0; i < 5; i++)
                Lines[2, i] = "─";
        }
    }
}

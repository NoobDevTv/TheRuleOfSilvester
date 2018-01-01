using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerLeftUp : Cell
    {
        public CornerLeftUp(Map map) : base(map)
        {
            Lines[0, 4] = "│";
            Lines[0, 0] = "┘";
            Lines[1, 4] = "│";
            Lines[2, 4] = "┘";
            for (int i = 0; i < 4; i++)
                Lines[2, i] = "─";
        }
    }
}

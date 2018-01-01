using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class StraightUpDown : Cell
    {
        public StraightUpDown(Map map) : base(map)
        {
            for (int i = 0; i < 3; i++)
            {
                Lines[i, 0] = "│";
                Lines[i, 4] = "│";
            }
        }
    }
}

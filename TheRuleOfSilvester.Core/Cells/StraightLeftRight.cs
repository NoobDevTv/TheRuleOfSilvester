using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class StraightLeftRight : Cell
    {
        public StraightLeftRight(Map map) : base(map)
        {
            for (int i = 0; i < 5; i++)
            {
                Lines[0, i] = "─";
                Lines[2, i] = "─";
            }
        }
    }
}

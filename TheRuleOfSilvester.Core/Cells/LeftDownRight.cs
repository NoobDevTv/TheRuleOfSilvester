using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    class LeftDownRight : Cell
    {
        public LeftDownRight(Map map) : base(map)
        {
            Lines[2, 0] = "┐";
            Lines[2, 4] = "┌";
            for (int i = 0; i < 5; i++)
                Lines[0, i] = "─";
        }
    }
}

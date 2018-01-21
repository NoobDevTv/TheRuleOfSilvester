using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CrossLeftRightUpDown : Cell
    {
        public CrossLeftRightUpDown(Map map) : base(map)
        {
            Lines[0, 0] = "┘";
            Lines[0, 4] = "└";
            Lines[2, 0] = "┐";
            Lines[2, 4] = "┌";
        }
    }
}

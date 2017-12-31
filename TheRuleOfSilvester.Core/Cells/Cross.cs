using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class Cross : Cell
    {
        public Cross() : base()
        {
            Lines[0, 0] = "┘";
            Lines[0, 4] = "└";
            Lines[2, 0] = "┐";
            Lines[2, 4] = "┌";
        }
    }
}

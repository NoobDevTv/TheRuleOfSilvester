using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerLeftUp : MapCell
    {
        public CornerLeftUp(Map map) : base(map)
        {
            Lines[4, 0] = '│';
            Lines[0, 0] = '┘';
            Lines[4, 1] = '│';
            Lines[4, 2] = '┘';
            for (int i = 0; i < 4; i++)
                Lines[i, 2] = '─';
        }
    }
}

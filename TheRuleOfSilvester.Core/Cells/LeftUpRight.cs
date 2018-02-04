using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    class LeftUpRight : MapCell
    {
        public LeftUpRight(Map map) : base(map)
        {
            Lines[0, 0] = '┘';
            Lines[4, 0] = '└';
            for (int i = 0; i < 5; i++)
                Lines[i, 2] = '─';
        }
    }
}

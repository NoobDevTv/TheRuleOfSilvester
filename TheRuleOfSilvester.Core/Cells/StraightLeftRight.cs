using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class StraightLeftRight : MapCell
    {
        public StraightLeftRight(Map map, bool movable = true) : base(map, movable)
        {
            for (int i = 0; i < 5; i++)
            {
                Lines[i, 0] = Movable ? '─' : '═';
                Lines[i, 2] = Movable ? '─' : '═';
            }
        }
    }
}

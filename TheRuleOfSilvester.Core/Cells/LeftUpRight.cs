using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    class LeftUpRight : MapCell
    {
        public LeftUpRight(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 0] = Movable ? '┘' : '╝';
            Lines[4, 0] = Movable ? '└' : '╚';
            for (int i = 0; i < 5; i++)
                Lines[i, 2] = Movable ? '─' : '═';
        }
    }
}

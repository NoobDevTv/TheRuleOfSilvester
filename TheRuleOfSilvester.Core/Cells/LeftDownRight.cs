using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    class LeftDownRight : MapCell
    {
        public LeftDownRight(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 2] = Movable ? '┐' : '╗';
            Lines[4, 2] = Movable ? '┌' : '╔';
            for (int i = 0; i < 5; i++)
                Lines[i, 0] = Movable ? '─' : '═';
        }
    }
}

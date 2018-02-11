using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerLeftUp : MapCell
    {
        public CornerLeftUp(Map map, bool movable = true) : base(map, movable)
        {

            Lines[4, 0] =   Movable ? '│' : '║';
            Lines[0, 0] = Movable ? '┘' : '╝';
            Lines[4, 1] = Movable ? '│' : '║';
            Lines[4, 2] = Movable ? '┘' : '╝';
            for (int i = 0; i < 4; i++)
                Lines[i, 2] = Movable ? '─' : '═';
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    class UpDownLeft : MapCell
    {
        public UpDownLeft(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 0] = Movable ? '┘' : '╝';
            Lines[0, 2] = Movable ? '┐' : '╗';
            for (int i = 0; i < 3; i++)
                Lines[4, i] = Movable ? '│' : '║';
        }
    }
}

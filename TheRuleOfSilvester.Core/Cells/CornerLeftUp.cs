using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("726D1EE9-90C5-48F1-A206-2EAEB19AC24B")]
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

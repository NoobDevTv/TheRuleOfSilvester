using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.Cells
{
    [Guid("726D1EE9-90C5-48F1-A206-2EAEB19AC24B")]
    public class CornerLeftUp : MapCell
    {
        public override string CellName => nameof(CornerLeftUp);
        public override ConnectionPoint ConnectionPoint => ConnectionPoint.Left | ConnectionPoint.Up;

        public CornerLeftUp(Map map, bool movable = true) : base(map, movable)
        {

            Lines[4, 0] = Movable ? '│' : '║';
            Lines[0, 0] = Movable ? '┘' : '╝';
            Lines[4, 1] = Movable ? '│' : '║';
            Lines[4, 2] = Movable ? '┘' : '╝';
            for (var i = 0; i < 4; i++)
                Lines[i, 2] = Movable ? '─' : '═';
        }
    }
}

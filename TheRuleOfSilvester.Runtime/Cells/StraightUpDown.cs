using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.Cells
{
    [Guid("5729128C-5D6E-4ABA-BCC6-F6190D1F7FF0")]
    public class StraightUpDown : MapCell
    {
        public override string CellName => nameof(StraightUpDown);

        public override ConnectionPoint ConnectionPoint => ConnectionPoint.Up | ConnectionPoint.Down;

        public StraightUpDown(Map map, bool movable = true) : base(map, movable)
        {
            for (var i = 0; i < 3; i++)
            {
                Lines[0, i] = Movable ? '│' : '║';
                Lines[4, i] = Movable ? '│' : '║';
            }
        }
    }
}

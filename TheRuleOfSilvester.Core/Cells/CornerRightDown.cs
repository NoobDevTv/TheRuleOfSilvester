using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("0D85862D-0B7D-4613-A513-F47EE3E6F8D7")]
    public class CornerRightDown : MapCell
    {
        public override string CellName => nameof(CornerRightDown);
        public override ConnectionPoint ConnectionPoint => ConnectionPoint.Right | ConnectionPoint.Down;

        public CornerRightDown(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 2] = Movable ? '│' : '║';
            Lines[4, 2] = Movable ? '┌' : '╔';
            Lines[0, 1] = Movable ? '│' : '║';
            Lines[0, 0] = Movable ? '┌' : '╔';
            for (var i = 1; i < 5; i++)
                Lines[i, 0] = Movable ? '─' : '═';
        }
    }
}

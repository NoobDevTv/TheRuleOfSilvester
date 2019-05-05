using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("67861DA4-9D1C-42B0-9B11-85516B908F75")]
    internal class LeftDownRight : MapCell
    {
        public override string CellName => nameof(LeftDownRight);

        public override ConnectionPoint ConnectionPoint => ConnectionPoint.Left | ConnectionPoint.Down | ConnectionPoint.Right;

        public LeftDownRight(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 2] = Movable ? '┐' : '╗';
            Lines[4, 2] = Movable ? '┌' : '╔';
            for (var i = 0; i < 5; i++)
                Lines[i, 0] = Movable ? '─' : '═';
        }
    }
}

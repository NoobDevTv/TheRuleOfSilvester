using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("315958D8-3618-4CA7-A212-FB1288D5D7A0")]
    public class CrossLeftRightUpDown : MapCell
    {
        public override string CellName => nameof(CrossLeftRightUpDown);
        public override ConnectionPoint ConnectionPoint => ConnectionPoint.Left | ConnectionPoint.Right | ConnectionPoint.Up | ConnectionPoint.Down;

        public CrossLeftRightUpDown(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 0] = Movable ? '┘' : '╝';
            Lines[4, 0] = Movable ? '└' : '╚';
            Lines[0, 2] = Movable ? '┐' : '╗';
            Lines[4, 2] = Movable ? '┌' : '╔';
        }
    }
}

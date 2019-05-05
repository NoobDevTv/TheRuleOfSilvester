using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("A107CCB2-3802-41A5-9E47-AB4D175EE89A")]
    internal class LeftUpRight : MapCell
    {
        public override string CellName => nameof(LeftUpRight);

        public override ConnectionPoint ConnectionPoint => ConnectionPoint.Left | ConnectionPoint.Up | ConnectionPoint.Right;

        public LeftUpRight(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 0] = Movable ? '┘' : '╝';
            Lines[4, 0] = Movable ? '└' : '╚';
            for (var i = 0; i < 5; i++)
                Lines[i, 2] = Movable ? '─' : '═';
        }
    }
}

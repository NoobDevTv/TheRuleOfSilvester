using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("22A7CB27-2AD3-491D-BD2B-4A172EC768B6")]
    internal class UpDownLeft : MapCell
    {
        public override string CellName => nameof(UpDownLeft);

        public override ConnectionPoint ConnectionPoint => ConnectionPoint.Up | ConnectionPoint.Down | ConnectionPoint.Left;

        public UpDownLeft(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 0] = Movable ? '┘' : '╝';
            Lines[0, 2] = Movable ? '┐' : '╗';
            for (var i = 0; i < 3; i++)
                Lines[4, i] = Movable ? '│' : '║';
        }
    }
}

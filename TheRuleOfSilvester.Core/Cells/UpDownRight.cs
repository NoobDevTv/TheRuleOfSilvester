using System;
using System.Runtime.InteropServices;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("6971B8CC-9A66-45F4-98B4-6497A43F74E8")]
    internal class UpDownRight : MapCell
    {
        public override string CellName => nameof(UpDownRight);

        public override ConnectionPoint ConnectionPoint => ConnectionPoint.Up | ConnectionPoint.Down | ConnectionPoint.Right;

        public UpDownRight(Map map, bool movable = true) : base(map, movable)
        {
            Lines[4, 2] = Movable ? '┌' : '╔';
            Lines[4, 0] = Movable ? '└' : '╚';
            for (var i = 0; i < 3; i++)
                Lines[0, i] = Movable ? '│' : '║';
        }
    }
}

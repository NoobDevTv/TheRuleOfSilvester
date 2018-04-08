using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("6971B8CC-9A66-45F4-98B4-6497A43F74E8")]
    class UpDownRight : MapCell
    {

        public UpDownRight(Map map, bool movable = true) : base(map, movable)
        {
            Lines[4, 2] = Movable ? '┌' : '╔';
            Lines[4, 0] = Movable ? '└' : '╚';
            for (int i = 0; i < 3; i++)
                Lines[0, i] = Movable ? '│' : '║';
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
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

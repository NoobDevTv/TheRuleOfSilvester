using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CrossLeftRightUpDown : MapCell
    {
        public CrossLeftRightUpDown(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 0] = Movable ? '┘' : '╝';
            Lines[4, 0] = Movable ? '└' : '╚';
            Lines[0, 2] = Movable ? '┐' : '╗';
            Lines[4, 2] = Movable ? '┌' : '╔';
        }
    }
}

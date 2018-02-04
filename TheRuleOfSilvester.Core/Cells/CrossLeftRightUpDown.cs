using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CrossLeftRightUpDown : MapCell
    {
        public CrossLeftRightUpDown(Map map) : base(map)
        {
            Lines[0, 0] = '┘';
            Lines[4, 0] = '└';
            Lines[0, 2] = '┐';
            Lines[4, 2] = '┌';
        }
    }
}

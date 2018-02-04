using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    class UpDownLeft : MapCell
    {
        public UpDownLeft(Map map) : base(map)
        {
            Lines[0, 0] = '┘';
            Lines[0, 2] = '┐';
            for (int i = 0; i < 3; i++)
                Lines[4, i] = '│';
        }
    }
}

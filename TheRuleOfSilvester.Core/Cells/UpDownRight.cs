using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    class UpDownRight : MapCell
    {
        public UpDownRight(Map map) : base(map)
        {
            Lines[4, 2] = '┌';
            Lines[4, 0] = '└';
            for (int i = 0; i < 3; i++)
                Lines[0, i] = '│';
        }
    }
}

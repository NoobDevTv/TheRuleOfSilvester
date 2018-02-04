using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class StraightUpDown : MapCell
    {
        public StraightUpDown(Map map) : base(map)
        {
            for (int i = 0; i < 3; i++)
            {
                Lines[0, i] = '│';
                Lines[4, i] = '│';
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class StraightLeftRight : MapCell
    {
        public StraightLeftRight(Map map) : base(map)
        {
            for (int i = 0; i < 5; i++)
            {
                Lines[i, 0] = '─';
                Lines[i, 2] = '─';
            }
        }
    }
}

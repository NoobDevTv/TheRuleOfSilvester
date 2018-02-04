using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerRightUp : MapCell
    {
        public CornerRightUp(Map map) : base(map)
        {

            Lines[0, 0] = '│';
            Lines[4, 0] = '└';
            Lines[0, 1] = '│';
            Lines[0, 2] = '└';
            for (int i = 1; i < 5; i++)
                Lines[i, 2] = '─'; 
        }
    }
}

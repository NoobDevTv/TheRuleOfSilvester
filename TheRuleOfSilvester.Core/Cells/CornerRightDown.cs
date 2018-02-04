using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerRightDown : MapCell
    {
        public CornerRightDown(Map map) : base(map)
        {
            Lines[0, 2] = '│';
            Lines[4, 2] = '┌';
            Lines[0, 1] = '│';
            Lines[0, 0] = '┌';
            for (int i = 1; i < 5; i++)
                Lines[i, 0] = '─';
        }
    }
}

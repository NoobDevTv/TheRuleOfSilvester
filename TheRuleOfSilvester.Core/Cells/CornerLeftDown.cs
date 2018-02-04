using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerLeftDown : MapCell
    {
        public CornerLeftDown(Map map) : base(map)
        {
            Lines[4, 2] = '│';
            Lines[0, 2] = '┐';
            Lines[4, 1] = '│';
            Lines[4, 0] = '┐';
            for (int i = 0; i < 4; i++)
                Lines[i, 0] = '─';
        }
    }
}

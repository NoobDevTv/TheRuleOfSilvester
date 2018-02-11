using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class StraightUpDown : MapCell
    {
        public StraightUpDown(Map map, bool movable = true) : base(map, movable)
        {
            for (int i = 0; i < 3; i++)
            {
                Lines[0, i] = Movable ? '│' : '║';
                Lines[4, i] = Movable ? '│' : '║';
            }
        }
    }
}

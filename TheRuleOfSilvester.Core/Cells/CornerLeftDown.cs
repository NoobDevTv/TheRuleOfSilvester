using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerLeftDown : MapCell
    {
        public CornerLeftDown(Map map, bool movable = true) : base(map, movable)
        {
            Lines[4, 2] = Movable ? '│' : '║';
            Lines[0, 2] = Movable ? '┐' : '╗';
            Lines[4, 1] = Movable ? '│' : '║';
            Lines[4, 0] = Movable ? '┐' : '╗';
            for (int i = 0; i < 4; i++)
                Lines[i, 0] = Movable ? '─' : '═';
        }
    }
}

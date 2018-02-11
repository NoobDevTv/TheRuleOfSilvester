using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class CornerRightDown : MapCell
    {
        public CornerRightDown(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 2] = Movable ? '│' : '║';
            Lines[4, 2] = Movable ? '┌' : '╔';
            Lines[0, 1] = Movable ? '│' : '║';
            Lines[0, 0] = Movable ? '┌' : '╔';
            for (int i = 1; i < 5; i++)
                Lines[i, 0] = Movable ? '─' : '═';
        }
    }
}

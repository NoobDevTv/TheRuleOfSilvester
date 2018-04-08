using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("5729128C-5D6E-4ABA-BCC6-F6190D1F7FF0")]
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

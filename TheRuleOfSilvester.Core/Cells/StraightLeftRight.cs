using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("029E61FD-51E0-4B16-A1A1-3B6BBA534E3C")]
    public class StraightLeftRight : MapCell
    {

        public StraightLeftRight(Map map, bool movable = true) : base(map, movable)
        {
            for (int i = 0; i < 5; i++)
            {
                Lines[i, 0] = Movable ? '─' : '═';
                Lines[i, 2] = Movable ? '─' : '═';
            }
        }
    }
}

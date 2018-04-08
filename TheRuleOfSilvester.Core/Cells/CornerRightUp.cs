using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    [Guid("A0D9A44A-EF84-4E5A-929E-2664E51F481C")]
    public class CornerRightUp : MapCell
    {

        public CornerRightUp(Map map, bool movable = true) : base(map, movable)
        {

            Lines[0, 0] = Movable ? '│' : '║';
            Lines[4, 0] = Movable ? '└' : '╚';
            Lines[0, 1] = Movable ? '│' : '║';
            Lines[0, 2] = Movable ? '└' : '╚';
            for (int i = 1; i < 5; i++)
                Lines[i, 2] = Movable ? '─' : '═'; 
        }
    }
}

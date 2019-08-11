using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TheRuleOfSilvester.Runtime.Cells;

namespace TheRuleOfSilvester.Runtime.Items
{
    [Guid("14165DEE-CE9D-4FE1-B73B-E3F29DEE2449")]
    public class CoinStack : BaseItemCell
    {
        public CoinStack(Map map, bool movable = true) : base(map, movable)
        {
            Lines[0, 0] = '⛃';
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Items
{
    public class BaseItemCell : Cell
    {
        public BaseItemCell(Map map, bool movable = true) : base(map, movable)
        {
        }

        public BaseItemCell(int width, int height, Map map, bool movable = true) : base(width, height, map, movable)
        {
        }
    }
}

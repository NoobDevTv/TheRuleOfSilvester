using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public abstract class MapCell : Cell
    {
        public MapCell(Map map) : base(5, 3, map)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public abstract class MapCell : Cell
    {
        public MapCell(Map map, bool movable = true) : base(5, 3, map, movable)
        {
            //TODO Here we can find it
            //Lines[9, 9] = new OurStruct(32768 /*= '|'*/, CellDirections.UpConnected | CellDirections.DownConnected);
        }
    }
}

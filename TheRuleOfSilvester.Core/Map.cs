using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core
{
    public class Map
    {
        //┌┬┐└┴┘─│├┼┤
        //╔╦╗╚╩╝═║╠╬╣
        public Cell[,] Cells { get; set; }
        public Map()
        {
            Cells = new Cell[10, 24];


            Cells[0, 0] = new CornerRightDown();
            Cells[0, 1] = new LeftDownRight();
            Cells[0, 2] = new CornerLeftDown();
            Cells[1, 0] = new UpDownRight();
            Cells[1, 1] = new Cross();
            Cells[1, 2] = new UpDownLeft();
            Cells[2, 0] = new CornerRightUp();
            Cells[2, 1] = new LeftUpRight();
            Cells[2, 2] = new CornerLeftUp();
        }
    }
}

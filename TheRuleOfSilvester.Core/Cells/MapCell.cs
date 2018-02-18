using System;
using System.Collections.Generic;
using System.Linq;
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

        public void NormalizeLayering()
        {
            var nTopCell = Map.Cells.FirstOrDefault(c => c.Position.X == Position.X && c.Position.Y == Position.Y - 1);
            var nDownCell = Map.Cells.FirstOrDefault(c => c.Position.X == Position.X && c.Position.Y == Position.Y + 1);
            var nLeftCell = Map.Cells.FirstOrDefault(c => c.Position.X == Position.X - 1 && c.Position.Y == Position.Y);
            var nRightCell = Map.Cells.FirstOrDefault(c => c.Position.X == Position.X + 1 && c.Position.Y == Position.Y);

            //var ourCell = map.Cells.FirstOrDefault(c => c.Position.X == tempX && c.Position.Y == tempY);
            if (nTopCell != null)
            {
                Lines[0, 0] = Lines[0, 0].Connections | ((nTopCell.Lines[0, 2].Connections & ConnectionPoints.Down) == ConnectionPoints.Down ? ConnectionPoints.Up : ConnectionPoints.None);
                Lines[4, 0] = Lines[4, 0].Connections | ((nTopCell.Lines[4, 2].Connections & ConnectionPoints.Down) == ConnectionPoints.Down ? ConnectionPoints.Up : ConnectionPoints.None);
            }
            if (nDownCell != null)
            {
                Lines[0, 2] = Lines[0, 2].Connections | ((nDownCell.Lines[0, 0].Connections & ConnectionPoints.Up) == ConnectionPoints.Up ? ConnectionPoints.Down : ConnectionPoints.None);
                Lines[4, 2] = Lines[4, 2].Connections | ((nDownCell.Lines[4, 0].Connections & ConnectionPoints.Up) == ConnectionPoints.Up ? ConnectionPoints.Down : ConnectionPoints.None);

            }
            if (nLeftCell != null)
            {
                Lines[0, 0] = Lines[0, 0].Connections | ((nLeftCell.Lines[4, 0].Connections & ConnectionPoints.Right) == ConnectionPoints.Right ? ConnectionPoints.Left : ConnectionPoints.None);
                Lines[0, 2] = Lines[0, 2].Connections | ((nLeftCell.Lines[4, 2].Connections & ConnectionPoints.Right) == ConnectionPoints.Right ? ConnectionPoints.Left : ConnectionPoints.None);

            }
            if (nRightCell != null)
            {
                Lines[4, 0] = Lines[4, 0].Connections | ((nRightCell.Lines[0, 0].Connections & ConnectionPoints.Left) == ConnectionPoints.Left ? ConnectionPoints.Right : ConnectionPoints.None);
                Lines[4, 2] = Lines[4, 2].Connections | ((nRightCell.Lines[0, 2].Connections & ConnectionPoints.Left) == ConnectionPoints.Left ? ConnectionPoints.Right : ConnectionPoints.None);

            }

            if (!Movable)
                foreach (var item in Lines)
                    if (item != null && item.ElementID % 2 == 1)
                        item.ElementID++;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core
{
    public class MapGenerator : BasicMapGenerator
    {
        public List<Type> CellTypes { get; private set; }

        public MapGenerator()
        {
            CellTypes = new List<Type>();

            CellTypes.AddRange(Assembly.GetExecutingAssembly()
                 .GetTypes()
                 .Where(c => c.Namespace == "TheRuleOfSilvester.Core.Cells" //TODO: variable
                 && c.BaseType == typeof(Cell)));
        }

        public override Map Generate(int x, int y)
        {
            var map = new Map(x, y);
            map.Cells.Clear();

            var tileSet = BuildTileSet();

            var topCells = GetCellsWithDirection(tileSet, 1, 0);
            var downCells = GetCellsWithDirection(tileSet, 1, 2);
            var leftCells = GetCellsWithDirection(tileSet, 0, 0);
            var rightCells = GetCellsWithDirection(tileSet, 0, 4);

            map.Cells.Add(new CornerRightDown(map) { Position = new Point(0, 0) });
            map.Cells.Add(new CornerLeftDown(map) { Position = new Point(0, y) });
            map.Cells.Add(new CornerRightUp(map) { Position = new Point(x, 0) });
            map.Cells.Add(new CornerLeftUp(map) { Position = new Point(x, y) });

            //Todo should be more generic

            for (int i = 1; i < y; i++)
            {
                var cell = (Cell)Activator.CreateInstance(topCells[random.Next(0, topCells.Count)].GetType(), map);
                cell.Position = new Point(0, i);
                map.Cells.Add(cell);
            }

            for (int i = 1; i < x; i++)
            {
                var cell = (Cell)Activator.CreateInstance(leftCells[random.Next(0, topCells.Count)].GetType(), map);
                cell.Position = new Point(i, 0);
                map.Cells.Add(cell);
            }

            for (int i = 1; i < y; i++)
            {
                var cell = (Cell)Activator.CreateInstance(downCells[random.Next(0, topCells.Count)].GetType(), map);
                cell.Position = new Point(x, i);
                map.Cells.Add(cell);
            }

            for (int i = 1; i < x; i++)
            {
                var cell = (Cell)Activator.CreateInstance(rightCells[random.Next(0, topCells.Count)].GetType(), map);
                cell.Position = new Point(i, y);
                map.Cells.Add(cell);
            }

            return map;
        }

        /// <summary>
        /// Gets cells with specific direction
        /// Top: 1, 0
        /// Down: 1, 2
        /// Left: 0, 0
        /// Right: 0, 4
        /// </summary>
        /// <param name="tileSet"></param>
        /// <param name="topRight"></param>
        /// <param name="downLeft"></param>
        /// <returns>Cell with specific direction</returns>
        private List<Cell> GetCellsWithDirection(Cell[] tileSet, byte topRight, byte downLeft)
        {
            var tmpList = new List<Cell>();

            foreach (var tile in tileSet)
            {
                var length = tile.Lines.GetLength(topRight);
                bool containsSpace = false;

                for (int i = 0; i < length; i++)
                {
                    if (tile.Lines[topRight == 0 ? i : downLeft, topRight == 0 ? downLeft : i] == null)
                    {
                        containsSpace = true;
                        break;
                    }
                }

                if (containsSpace)
                    continue;
                else
                    tmpList.Add(tile);
            }

            return tmpList;
        }

        public Cell[] BuildTileSet()
        {
            var tmp = new Cell[CellTypes.Count];

            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = (Cell)Activator.CreateInstance(CellTypes[i], (Map)null);
            }

            return tmp;
        }
    }
}

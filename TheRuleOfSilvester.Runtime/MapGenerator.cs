using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Runtime.Cells;
using TheRuleOfSilvester.Runtime.Items;

namespace TheRuleOfSilvester.Runtime
{
    public class MapGenerator : BasicMapGenerator
    {
        public MapGenerator()
        {
            CellTypes = new List<Type>();

            CellTypes.AddRange(Assembly.GetExecutingAssembly()
                 .GetTypes()
                 .Where(c => c.BaseType == typeof(MapCell)));
        }

        public MapGenerator(Guid[] cellGUIDs) : base(cellGUIDs)
        {
        }

        public override Map Generate(int x, int y)
        {
            var map = new Map(x, y, this);
            map.Cells.Clear();
            MapCell[,] mapCells = new MapCell[x, y];


            var localCellTypes = CellTypes.Select(t => new
            {
                Type = t,
                ((MapCell)Activator.CreateInstance(t, map, true)).ConnectionPoint
            }).ToList();

            var topCells = localCellTypes.Where(ct => !((ct.ConnectionPoint & ConnectionPoint.Up) > 0)).Select(c => c.Type).ToList();
            var downCells = localCellTypes.Where(ct => !((ct.ConnectionPoint & ConnectionPoint.Down) > 0)).Select(c => c.Type).ToList();
            var leftCells = localCellTypes.Where(ct => !((ct.ConnectionPoint & ConnectionPoint.Left) > 0)).Select(c => c.Type).ToList();
            var rightCells = localCellTypes.Where(ct => !((ct.ConnectionPoint & ConnectionPoint.Right) > 0)).Select(c => c.Type).ToList();



            var maxX = x - 1;
            var maxY = y - 1;

            mapCells[0, 0] = new CornerRightDown(map, false) { Position = new Position(0, 0) };
            mapCells[maxX, 0] = new CornerLeftDown(map, false) { Position = new Position(maxX, 0) };
            mapCells[0, maxY] = new CornerRightUp(map, false) { Position = new Position(0, maxY) };
            mapCells[maxX, maxY] = new CornerLeftUp(map, false) { Position = new Position(maxX, maxY) };


            //Todo should be more generic
            for (var i = 1; i < maxX; i++)
            {
                var cell = (MapCell)Activator.CreateInstance(topCells[random.Next(0, topCells.Count)], map, false);
                cell.Position = new Position(i, 0);
                mapCells[i, 0] = cell;

            }

            for (var i = 1; i < maxY; i++)
            {
                var cell = (MapCell)Activator.CreateInstance(leftCells[random.Next(0, topCells.Count)], map, false);
                cell.Position = new Position(0, i);
                cell.Movable = false;
                mapCells[0, i] = cell;
            }

            for (var i = 1; i < maxX; i++)
            {
                var cell = (MapCell)Activator.CreateInstance(downCells[random.Next(0, topCells.Count)], map, false);
                cell.Position = new Position(i, maxY);
                cell.Movable = false;
                mapCells[i, maxY] = cell;
            }

            for (var i = 1; i < maxY; i++)
            {
                var cell = (MapCell)Activator.CreateInstance(rightCells[random.Next(0, topCells.Count)], map, false);
                cell.Position = new Position(maxX, i);
                cell.Movable = false;
                mapCells[maxX, i] = cell;
            }

            //x + 1 bis maxX
            //y + 1 bis maxY
            for (var tempX = 1; tempX < maxX; tempX++)
            {
                for (var tempY = 1; tempY < maxY; tempY++)
                {
                    bool nTopCell = (mapCells[tempX, tempY - 1]?.ConnectionPoint & ConnectionPoint.Down) > 0;
                    bool nDownCell = (mapCells[tempX, tempY + 1]?.ConnectionPoint & ConnectionPoint.Up) > 0;
                    bool nLeftCell = (mapCells[tempX - 1, tempY]?.ConnectionPoint & ConnectionPoint.Right) > 0;
                    bool nRightCell = (mapCells[tempX + 1, tempY]?.ConnectionPoint & ConnectionPoint.Left) > 0;

                    var possibleCells = localCellTypes.Where(cellType =>
                                                (nTopCell ? (((cellType.ConnectionPoint & ConnectionPoint.Up) > 0) ? true : false) : true)
                                            && (nDownCell ? (((cellType.ConnectionPoint & ConnectionPoint.Down) > 0) ? true : false) : true)
                                            && (nLeftCell ? (((cellType.ConnectionPoint & ConnectionPoint.Left) > 0) ? true : false) : true)
                                            && (nRightCell ? (((cellType.ConnectionPoint & ConnectionPoint.Right) > 0) ? true : false) : true))
                                .Select(c => c.Type).ToArray();

                    var cell = (MapCell)Activator.CreateInstance(possibleCells[random.Next(0, possibleCells.Length)], map, true);
                    cell.Position = new Position(tempX, tempY);

                    mapCells[tempX, tempY] = cell;
                    //localMapCells.Add(cell);
                }
            }

            var localMapCells = new List<MapCell>(x * y);
            foreach (var cell in mapCells)
                localMapCells.Add(cell);

            //Parallel.ForEach(localMapCells, ourCell => (ourCell as MapCell).NormalizeLayering(localMapCells));

            map.Cells = localMapCells.OfType<Cell>().ToList();
            //Default coin stack
            map.Cells.Add(new CoinStack(map) { Position = new Position(5 * random.Next(1, map.Width - 2) - 3, 3 * random.Next(1, map.Height - 2) - 2) });

            return map;
        }
    }
}

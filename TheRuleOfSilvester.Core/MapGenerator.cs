﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core
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

            //var tileSet = BuildTileSet();

            var topCells = CellTypes.Where(ct => !ct.Name.ToLower().Contains("up")).ToList();
            var downCells = CellTypes.Where(ct => !ct.Name.ToLower().Contains("down")).ToList();
            var leftCells = CellTypes.Where(ct => !ct.Name.ToLower().Contains("left")).ToList();
            var rightCells = CellTypes.Where(ct => !ct.Name.ToLower().Contains("right")).ToList();

            map.Cells.Add(new CornerRightDown(map, false) { Position = new Point(0, 0) });
            map.Cells.Add(new CornerLeftDown(map, false) { Position = new Point(x, 0) });
            map.Cells.Add(new CornerRightUp(map, false) { Position = new Point(0, y) });
            map.Cells.Add(new CornerLeftUp(map, false) { Position = new Point(x, y) });

            //Todo should be more generic

            for (int i = 1; i < x; i++)
            {
                var cell = (Cell)Activator.CreateInstance(topCells[random.Next(0, topCells.Count)], map, false);
                cell.Position = new Point(i, 0);
                map.Cells.Add(cell);
            }

            for (int i = 1; i < y; i++)
            {
                var cell = (Cell)Activator.CreateInstance(leftCells[random.Next(0, topCells.Count)], map, false);
                cell.Position = new Point(0, i);
                cell.Movable = false;
                map.Cells.Add(cell);
            }

            for (int i = 1; i < x; i++)
            {
                var cell = (Cell)Activator.CreateInstance(downCells[random.Next(0, topCells.Count)], map, false);
                cell.Position = new Point(i, y);
                cell.Movable = false;
                map.Cells.Add(cell);
            }

            for (int i = 1; i < y; i++)
            {
                var cell = (Cell)Activator.CreateInstance(rightCells[random.Next(0, topCells.Count)], map, false);
                cell.Position = new Point(x, i);
                cell.Movable = false;
                map.Cells.Add(cell);
            }
            //x + 1 bis x - 1
            //y + 1 bis y - 1

            for (int tempX = 1; tempX < x; tempX++)
                for (int tempY = 1; tempY < y; tempY++)
                {
                    var nTopCell = map.Cells.FirstOrDefault(c => c.Position.X == tempX && c.Position.Y == tempY - 1);
                    var nDownCell = map.Cells.FirstOrDefault(c => c.Position.X == tempX && c.Position.Y == tempY + 1);
                    var nLeftCell = map.Cells.FirstOrDefault(c => c.Position.X == tempX - 1 && c.Position.Y == tempY);
                    var nRightCell = map.Cells.FirstOrDefault(c => c.Position.X == tempX + 1 && c.Position.Y == tempY);

                    var possibleCells = CellTypes.Where(cellType => ((nTopCell != null
                                    && nTopCell.GetType().Name.ToLower().Contains("down")) ? (cellType.Name.ToLower().Contains("up") ? true : false) : true)
                                    && ((nDownCell != null && nDownCell.GetType().Name.ToLower().Contains("up")) ? (cellType.Name.ToLower().Contains("down") ? true : false) : true)
                                    && ((nLeftCell != null && nLeftCell.GetType().Name.ToLower().Contains("right")) ? (cellType.Name.ToLower().Contains("left") ? true : false) : true)
                                    && ((nRightCell != null && nLeftCell.GetType().Name.ToLower().Contains("left")) ? (cellType.Name.ToLower().Contains("right") ? true : false) : true)).ToArray();

                    var cell = (Cell)Activator.CreateInstance(possibleCells[random.Next(0, possibleCells.Length)], map, true);
                    cell.Position = new Point(tempX, tempY);
                    map.Cells.Add(cell);
                }

            foreach (MapCell ourCell in map.Cells)
                ourCell.NormalizeLayering();

            return map;
        }
    }
}

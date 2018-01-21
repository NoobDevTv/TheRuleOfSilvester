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

            var topCells = CellTypes.Where(ct => !ct.Name.ToLower().Contains("up")).ToList();
            var downCells = CellTypes.Where(ct => !ct.Name.ToLower().Contains("down")).ToList();
            var leftCells = CellTypes.Where(ct => !ct.Name.ToLower().Contains("left")).ToList();
            var rightCells = CellTypes.Where(ct => !ct.Name.ToLower().Contains("right")).ToList();

            map.Cells.Add(new CornerRightDown(map) { Position = new Point(0, 0) });
            map.Cells.Add(new CornerLeftDown(map) { Position = new Point(0, y) });
            map.Cells.Add(new CornerRightUp(map) { Position = new Point(x, 0) });
            map.Cells.Add(new CornerLeftUp(map) { Position = new Point(x, y) });

            //Todo should be more generic

            for (int i = 1; i < y; i++)
            {
                var cell = (Cell)Activator.CreateInstance(topCells[random.Next(0, topCells.Count)], map);
                cell.Position = new Point(0, i);
                map.Cells.Add(cell);
            }

            for (int i = 1; i < x; i++)
            {
                var cell = (Cell)Activator.CreateInstance(leftCells[random.Next(0, topCells.Count)], map);
                cell.Position = new Point(i, 0);
                map.Cells.Add(cell);
            }

            for (int i = 1; i < y; i++)
            {
                var cell = (Cell)Activator.CreateInstance(downCells[random.Next(0, topCells.Count)], map);
                cell.Position = new Point(x, i);
                map.Cells.Add(cell);
            }

            for (int i = 1; i < x; i++)
            {
                var cell = (Cell)Activator.CreateInstance(rightCells[random.Next(0, topCells.Count)], map);
                cell.Position = new Point(i, y);
                map.Cells.Add(cell);
            }
            //x + 1 bis x - 1
            //y + 1 bis y - 1

            for (int tempX = 1; tempX < x; tempX++)
            {
                for (int tempY = 1; tempY < y; tempY++)
                {
                    var nTopCell = map.Cells.FirstOrDefault(c => c.Position.X == tempX - 1 && c.Position.Y == tempY);
                    var nDownCell = map.Cells.FirstOrDefault(c => c.Position.X == tempX + 1 && c.Position.Y == tempY);
                    var nLeftCell = map.Cells.FirstOrDefault(c => c.Position.X == tempX && c.Position.Y == tempY - 1);
                    var nRightCell = map.Cells.FirstOrDefault(c => c.Position.X == tempX && c.Position.Y == tempY + 1);

                    var attributeList = new List<Direction>();

                    var attribute = GetOppositeDirection(nTopCell);

                    if (attribute != Direction.none)
                        attributeList.Add(attribute);

                    attribute = GetOppositeDirection(nDownCell);

                    if (attribute != Direction.none)
                        attributeList.Add(attribute);

                    attribute = GetOppositeDirection(nRightCell);

                    if (attribute != Direction.none)
                        attributeList.Add(attribute);

                    attribute = GetOppositeDirection(nLeftCell);

                    if (attribute != Direction.none)
                        attributeList.Add(attribute);
                    // var possipleCells = CellTypes.Where(asd => (nTopCell != null && asd.Name.ToLower().Contains("down"))
                    //|| (nDownCell != null && asd.Name.ToLower().Contains("up"))
                    //|| (nLeftCell != null && asd.Name.ToLower().Contains("right"))
                    //|| (nRightCell != null && asd.Name.ToLower().Contains("left"))).ToArray();

                    var possipleCells = CellTypes.Where(c =>
                    {
                        var result = true;
                        attributeList.ForEach(a =>
                            {
                                if (result)
                                    result = c.Name.Contains(a.ToString());
                            });
                        return result;
                    }).ToArray();
                    var cell = (Cell)Activator.CreateInstance(possipleCells[random.Next(0, possipleCells.Length)], map);
                    cell.Position = new Point(tempX, tempY);
                    map.Cells.Add(cell);

                }
            }

          
            return map;
        }

        private Direction GetOppositeDirection(Cell nTopCell)
        {
            var returnValue = Direction.none;
            var name = nTopCell.GetType().Name.ToLower();

            if (name.Contains("up")) 
                returnValue = Direction.Top;
            
            if (name.Contains("down"))
            {
                if (returnValue == Direction.Top)
                    returnValue ==
            }
            if (name.Contains("left")) ;
            if (name.Contains("right")) ;

            return returnValue;

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

    public enum Direction
    {
        Left = 1,
        Down = 4,
   
        none = 0,
        Top = 8,
        TopDown = Top | Down 
    }
}

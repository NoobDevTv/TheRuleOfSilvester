using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public class MapGenerator : BasicMapGenerator
    {
        public override Map Generate(int x, int y)
        {
            var tmpArray = new Cell[y * x];
            var types = Assembly.GetExecutingAssembly()
                 .GetTypes()
                 .Where(c => c.Namespace == "TheRuleOfSilvester.Core.Cells" //TODO: variable
                 && c.BaseType == typeof(Cell))
                 .ToList();

            var map = new Map(x * 3, y * 5, tmpArray);
            for (int tmpX = 0; tmpX < x; tmpX++)
            {
                for (int tmpY = 0; tmpY < y; tmpY++)
                {
                    var tmpCell = (Cell)Activator.CreateInstance(types[random.Next(0, types.Count)], map);
                    tmpCell.Position = new Point(tmpX, tmpY);
                    tmpArray[tmpX + tmpY * x] = tmpCell;
                }
            }


            map.Cells = tmpArray.ToList();
            return map;
        }
    }
}

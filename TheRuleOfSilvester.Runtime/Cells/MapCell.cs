using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.Runtime.Cells
{
    public abstract class MapCell : Cell, IByteSerializable
    {
        public Guid Guid { get; private set; }
        public abstract string CellName { get; }
        public abstract ConnectionPoint ConnectionPoint { get; }

        public MapCell(Map map, bool movable = true) : base(5, 3, map, movable)
        {
            var guidVal = GetType().GetCustomAttribute<GuidAttribute>()?.Value;
            Guid = string.IsNullOrWhiteSpace(guidVal) ? Guid.NewGuid() : new Guid(guidVal);
        }


        public void NormalizeLayering(IEnumerable<MapCell> mapCells)
        {
            ClearLayer();

            var xMinus = Position.X - 1;
            var xAdd = Position.X + 1;
            var yMinus = Position.Y - 1;
            var yAdd = Position.Y + 1;
            var nTopCell = mapCells.FirstOrDefault(c => c.Position.X == Position.X && c.Position.Y == yMinus);
            var nDownCell = mapCells.FirstOrDefault(c => c.Position.X == Position.X && c.Position.Y == yAdd);
            var nLeftCell = mapCells.FirstOrDefault(c => c.Position.X == xMinus && c.Position.Y == Position.Y);
            var nRightCell = mapCells.FirstOrDefault(c => c.Position.X == xAdd && c.Position.Y == Position.Y);

            if (nLeftCell != null)
            {
                Layer[0, 0] = (Layer[0, 0] == null ? ConnectionPoint.None : Layer[0, 0].Connections) | Lines[0, 0].Connections | ((nLeftCell.Lines[4, 0].Connections & ConnectionPoint.Right) == ConnectionPoint.Right ? ConnectionPoint.Left : ConnectionPoint.None);
                Layer[0, 2] = (Layer[0, 2] == null ? ConnectionPoint.None : Layer[0, 2].Connections) | Lines[0, 2].Connections | ((nLeftCell.Lines[4, 2].Connections & ConnectionPoint.Right) == ConnectionPoint.Right ? ConnectionPoint.Left : ConnectionPoint.None);
            }
            if (nTopCell != null)
            {
                Layer[0, 0] = (Layer[0, 0] == null ? ConnectionPoint.None : Layer[0, 0].Connections) | Lines[0, 0].Connections | ((nTopCell.Lines[0, 2].Connections & ConnectionPoint.Down) == ConnectionPoint.Down ? ConnectionPoint.Up : ConnectionPoint.None);
                Layer[4, 0] = (Layer[4, 0] == null ? ConnectionPoint.None : Layer[4, 0].Connections) | Lines[4, 0].Connections | ((nTopCell.Lines[4, 2].Connections & ConnectionPoint.Down) == ConnectionPoint.Down ? ConnectionPoint.Up : ConnectionPoint.None);
            }
            if (nDownCell != null)
            {
                Layer[0, 2] = (Layer[0, 2] == null ? ConnectionPoint.None : Layer[0, 2].Connections) | Lines[0, 2].Connections | ((nDownCell.Lines[0, 0].Connections & ConnectionPoint.Up) == ConnectionPoint.Up ? ConnectionPoint.Down : ConnectionPoint.None);
                Layer[4, 2] = (Layer[4, 2] == null ? ConnectionPoint.None : Layer[4, 2].Connections) | Lines[4, 2].Connections | ((nDownCell.Lines[4, 0].Connections & ConnectionPoint.Up) == ConnectionPoint.Up ? ConnectionPoint.Down : ConnectionPoint.None);
            }
            if (nRightCell != null)
            {
                Layer[4, 0] = (Layer[4, 0] == null ? ConnectionPoint.None : Layer[4, 0].Connections) | Lines[4, 0].Connections | ((nRightCell.Lines[0, 0].Connections & ConnectionPoint.Left) == ConnectionPoint.Left ? ConnectionPoint.Right : ConnectionPoint.None);
                Layer[4, 2] = (Layer[4, 2] == null ? ConnectionPoint.None : Layer[4, 2].Connections) | Lines[4, 2].Connections | ((nRightCell.Lines[0, 2].Connections & ConnectionPoint.Left) == ConnectionPoint.Left ? ConnectionPoint.Right : ConnectionPoint.None);
            }

            if (!Movable)
            {
                foreach (var item in Layer)
                    if (item != null && item.ElementID % 2 == 1)
                        item.ElementID++;
            }

            Invalid = true;
        }
        public void NormalizeLayering()
            => NormalizeLayering(Map.Cells.OfType<MapCell>());

        public void ClearLayer()
        {
            Layer = new BaseElement[Width, Height];
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Guid.ToByteArray());
            binaryWriter.Write(Movable);
            binaryWriter.Write(Position.X);
            binaryWriter.Write(Position.Y);
            binaryWriter.Write(Color.ToArgb());
        }

        public void Deserialize(BinaryReader binaryReader)
            => throw new NotSupportedException("Use SerializeHelper.DeserializeMapCell instead ;)");
    }
}
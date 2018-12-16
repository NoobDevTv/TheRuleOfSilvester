using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TheRuleOfSilvester.Core.Interfaces;

namespace TheRuleOfSilvester.Core.Cells
{
    public abstract class MapCell : Cell, IByteSerializable
    {
        public Guid Guid { get; private set; }


        public MapCell(Map map, bool movable = true) : base(5, 3, map, movable)
        {
            var guidVal = GetType().GetCustomAttribute<GuidAttribute>()?.Value;
            Guid = string.IsNullOrWhiteSpace(guidVal) ? Guid.NewGuid() : new Guid(guidVal);
        }

        public void NormalizeLayering()
        {
            ClearLayer();
            var mapCells = Map.Cells.Where(x => typeof(MapCell).IsAssignableFrom(x.GetType()));

            var nTopCell = mapCells.FirstOrDefault(c => c.Position.X == Position.X && c.Position.Y == Position.Y - 1);
            var nDownCell = mapCells.FirstOrDefault(c => c.Position.X == Position.X && c.Position.Y == Position.Y + 1);
            var nLeftCell = mapCells.FirstOrDefault(c => c.Position.X == Position.X - 1 && c.Position.Y == Position.Y);
            var nRightCell = mapCells.FirstOrDefault(c => c.Position.X == Position.X + 1 && c.Position.Y == Position.Y);

            if (nLeftCell != null)
            {
                Layer[0, 0] = (Layer[0, 0] == null ? ConnectionPoints.None : Layer[0, 0].Connections) | Lines[0, 0].Connections | ((nLeftCell.Lines[4, 0].Connections & ConnectionPoints.Right) == ConnectionPoints.Right ? ConnectionPoints.Left : ConnectionPoints.None);
                Layer[0, 2] = (Layer[0, 2] == null ? ConnectionPoints.None : Layer[0, 2].Connections) | Lines[0, 2].Connections | ((nLeftCell.Lines[4, 2].Connections & ConnectionPoints.Right) == ConnectionPoints.Right ? ConnectionPoints.Left : ConnectionPoints.None);
            }
            if (nTopCell != null)
            {
                Layer[0, 0] = (Layer[0, 0] == null ? ConnectionPoints.None : Layer[0, 0].Connections) | Lines[0, 0].Connections | ((nTopCell.Lines[0, 2].Connections & ConnectionPoints.Down) == ConnectionPoints.Down ? ConnectionPoints.Up : ConnectionPoints.None);
                Layer[4, 0] = (Layer[4, 0] == null ? ConnectionPoints.None : Layer[4, 0].Connections) | Lines[4, 0].Connections | ((nTopCell.Lines[4, 2].Connections & ConnectionPoints.Down) == ConnectionPoints.Down ? ConnectionPoints.Up : ConnectionPoints.None);
            }
            if (nDownCell != null)
            {
                Layer[0, 2] = (Layer[0, 2] == null ? ConnectionPoints.None : Layer[0, 2].Connections) | Lines[0, 2].Connections | ((nDownCell.Lines[0, 0].Connections & ConnectionPoints.Up) == ConnectionPoints.Up ? ConnectionPoints.Down : ConnectionPoints.None);
                Layer[4, 2] = (Layer[4, 2] == null ? ConnectionPoints.None : Layer[4, 2].Connections) | Lines[4, 2].Connections | ((nDownCell.Lines[4, 0].Connections & ConnectionPoints.Up) == ConnectionPoints.Up ? ConnectionPoints.Down : ConnectionPoints.None);
            }
            if (nRightCell != null)
            {
                Layer[4, 0] = (Layer[4, 0] == null ? ConnectionPoints.None : Layer[4, 0].Connections) | Lines[4, 0].Connections | ((nRightCell.Lines[0, 0].Connections & ConnectionPoints.Left) == ConnectionPoints.Left ? ConnectionPoints.Right : ConnectionPoints.None);
                Layer[4, 2] = (Layer[4, 2] == null ? ConnectionPoints.None : Layer[4, 2].Connections) | Lines[4, 2].Connections | ((nRightCell.Lines[0, 2].Connections & ConnectionPoints.Left) == ConnectionPoints.Left ? ConnectionPoints.Right : ConnectionPoints.None);
            }

            if (!Movable)
                foreach (var item in Layer)
                    if (item != null && item.ElementID % 2 == 1)
                        item.ElementID++;

            Invalid = true;
        }

        public void ClearLayer()
        {
            Layer = new BaseElement[Layer.GetLength(0), Layer.GetLength(1)];
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
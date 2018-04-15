using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core
{
    public class Map : IByteSerializable
    {
        //┌┬┐└┴┘─│├┼┤
        //╔╦╗╚╩╝═║╠╬╣
        public List<Cell> Cells { get; set; }
        public List<TextCell> TextCells { get; set; }
        public List<Player> Players { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public BasicMapGenerator MapGenerator { get; private set; }
        
        public Map(int width, int height, BasicMapGenerator mapGenerator)
        {
            Players = new List<Player>();
            MapGenerator = mapGenerator;
            Height = height;
            Width = width;
            Cells = new List<Cell>();
            TextCells = new List<TextCell>();
        }
        public Map() : this(0, 0, null) { }

        public bool IsTileOccupied(Point pos)
        {
            var cellList = Cells.Where(x => x.GetType() != typeof(Player)).Where(x =>
                 x.Position.X * x.Width <= pos.X && (x.Position.X * x.Width + x.Width) > pos.X
                 && x.Position.Y * x.Height <= pos.Y && (x.Position.Y * x.Height + x.Height) > pos.Y);

            foreach (var cell in cellList)
            {
                if (cell.Lines[pos.X % cell.Width, pos.Y % cell.Height] != null)
                    return true;
            }

            return false;
        }
        public Cell GetTileAbsolutePos(Point pos)
        {
            return Cells.Where(x => x.GetType() != typeof(Player)).FirstOrDefault(x =>
                 x.Position.X * x.Width <= pos.X && (x.Position.X * x.Width + x.Width) > pos.X
                 && x.Position.Y * x.Height <= pos.Y && (x.Position.Y * x.Height + x.Height) > pos.Y);

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Height);
            writer.Write(Width);
            writer.Write(Cells.Count);

            foreach (MapCell cell in Cells.Where(x => typeof(MapCell).IsAssignableFrom(x.GetType())))
                cell.Serialize(writer);

            writer.Write(MapGenerator.CellTypes.Count);

            foreach (var item in MapGenerator.CellTypes)
                writer.Write(item.GUID.ToByteArray());

        }

        public void Deserialize(BinaryReader binaryReader)
        {
            Height = binaryReader.ReadInt32();
            Width = binaryReader.ReadInt32();

            SerializeHelper.Map = this;
            var length1= binaryReader.ReadInt32();
            for (int i = 0; i < length1; i++)
                Cells.Add(SerializeHelper.DeserializeMapCell(binaryReader));
            
            var stringList = new List<Guid>();
            var length = binaryReader.ReadInt32();
            for (int i = 0; i < length; i++)
                stringList.Add(new Guid(binaryReader.ReadBytes(16)));

            MapGenerator = new MapGenerator(stringList.ToArray());

            foreach (MapCell ourCell in Cells)
                ourCell.NormalizeLayering();
        }
    }
}

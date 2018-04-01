using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        public BasicMapGenerator MapGenerator { get; }

        public Map(int width, int height, BasicMapGenerator mapGenerator)
        {
            Players = new List<Player>();
            MapGenerator = mapGenerator;
            Height = height;
            Width = width;
            Cells = new List<Cell>
            {
                new CornerRightDown (this) { Position = new Point(0, 0) },
                new LeftDownRight   (this) { Position = new Point(1, 0) },
                new CornerLeftDown  (this) { Position = new Point(2, 0) },
                new UpDownRight     (this) { Position = new Point(0, 1) },
                new CrossLeftRightUpDown (this) { Position = new Point(1, 1) },
                new UpDownLeft      (this) { Position = new Point(2, 1) },
                new CornerRightUp   (this) { Position = new Point(0, 2) },
                new LeftUpRight     (this) { Position = new Point(1, 2) },
                new CornerLeftUp    (this) { Position = new Point(2, 2) }
            };
            TextCells = new List<TextCell>();
        }

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

            foreach (var cell in Cells)
                cell.Serialize(writer);

        }

        public void Deserialize(BinaryReader binaryReader)
        {
            Height = binaryReader.ReadInt32();
            Width= binaryReader.ReadInt32();

            for (int i = 0; i < binaryReader.ReadInt32(); i++)
            {
                var cell = new Cell(this);
                cell.Deserialize(binaryReader);
                Cells.Add(cell);
            }
                
        }
    }
}

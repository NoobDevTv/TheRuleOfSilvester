using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TheRuleOfSilvester.Runtime.Cells;
using TheRuleOfSilvester.Runtime;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Runtime.Items;
using TheRuleOfSilvester.Core.Extensions;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Observation;
using System.Reactive.Linq;

namespace TheRuleOfSilvester.Runtime
{
    public class Map : IByteSerializable
    {
        //┌┬┐└┴┘─│├┼┤
        //╔╦╗╚╩╝═║╠╬╣
        public List<Cell> Cells { get; set; }
        public List<TextCell> TextCells { get; set; }
        public List<PlayerCell> Players { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public BasicMapGenerator MapGenerator { get; private set; }

        public Map(int width, int height, BasicMapGenerator mapGenerator)
        {
            Players = new List<PlayerCell>();
            MapGenerator = mapGenerator;
            Height = height;
            Width = width;
            Cells = new List<Cell>();
            TextCells = new List<TextCell>();
        }
        public Map() : this(0, 0, null) { }

        public bool IsTileOccupied(Position pos)
        {
            var cellList = Cells.OfType<MapCell>().Where(x => Cell.IsOnPosition(pos, x));

            foreach (var cell in cellList)
            {
                if (cell.Lines[pos.X % cell.Width, pos.Y % cell.Height] != null)
                    return true;
            }

            return false;
        }
        public Cell GetTileAbsolutePos(Position pos)
            => Cells.OfType<MapCell>().FirstOrDefault(x => Cell.IsOnPosition(pos, x));

        public Cell SwapInventoryAndMapCell(Cell cell, Position position, int x = 5)
        {
            var mapCell = Cells.First(c => c.Position == position);

            cell.Position = position;
            Cells.Remove(mapCell);
            Cells.Add(cell);

            mapCell.Position = new Position(x, Height + 2);
            cell.Invalid = true;
            mapCell.Invalid = true;

            Cells.Where(c =>
                               c.Position.X == cell.Position.X && c.Position.Y == cell.Position.Y - 1
                            || c.Position.X == cell.Position.X && c.Position.Y == cell.Position.Y + 1
                            || c.Position.X == cell.Position.X - 1 && c.Position.Y == cell.Position.Y
                            || c.Position.X == cell.Position.X + 1 && c.Position.Y == cell.Position.Y)
                            .Select(c => (MapCell)c)
                            .ForEach(c => c.NormalizeLayering());



            return mapCell;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Height);
            writer.Write(Width);
            writer.Write(Cells.Count);

            foreach (IByteSerializable cell in Cells.Where(x => x is MapCell || x is BaseItemCell))
            {
                cell.Serialize(writer);
            }

            writer.Write(MapGenerator.CellTypes.Count);

            foreach (var item in MapGenerator.CellTypes)
                writer.Write(item.GUID.ToByteArray());

        }

        public void Deserialize(BinaryReader binaryReader)
        {
            Height = binaryReader.ReadInt32();
            Width = binaryReader.ReadInt32();

            SerializeHelper.Map = this;
            var length1 = binaryReader.ReadInt32();
            for (int i = 0; i < length1; i++)
                Cells.Add(SerializeHelper.DeserializeMapCell(binaryReader));

            var stringList = new List<Guid>();
            var length = binaryReader.ReadInt32();
            for (int i = 0; i < length; i++)
                stringList.Add(new Guid(binaryReader.ReadBytes(16)));

            MapGenerator = new MapGenerator(stringList.ToArray());

            foreach (MapCell ourCell in Cells.Where(c => typeof(MapCell).IsAssignableFrom(c.GetType())))
                ourCell.NormalizeLayering();
        }

        public IDisposable SubscribePlayerNotifications(IObservable<Notification> notifications) => notifications
            .Where(n => n.Type == NotificationType.PlayerNotification)
            .Select(n => n.Deserialize(SerializeHelper.Deserialize<Player>))
            .Subscribe(AddPlayer);

        public void AddPlayer(IPlayer player)
        {
            var tmpPlayer = player as Player;

            if (tmpPlayer.IsLocal)
                tmpPlayer.Color = Color.Red;

            tmpPlayer.Map = this;
            Players.Add(tmpPlayer);
        }
    }
}

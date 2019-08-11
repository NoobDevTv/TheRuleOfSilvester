using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Runtime.Cells;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Runtime.Items;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime
{
    public abstract class PlayerCell : Cell, IByteSerializable
    {
        public int Id { get; set; }
        public bool IsLocal { get; set; }
        public Inventory<MapCell> CellInventory { get; set; }
        public Inventory<BaseItemCell> ItemInventory { get; set; }

        public PlayerCell(int width, int height, Map map) : base(width, height, map, true)
        {
            CellInventory = new Inventory<MapCell>(3);
            ItemInventory = new Inventory<BaseItemCell>(10);
            IsLocal = true;
        }

        public virtual void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Id);
        }

        public virtual void Deserialize(BinaryReader binaryReader)
        {
            Id = binaryReader.ReadInt32();
        }

        public override bool Equals(object obj)
        {
            if (obj is PlayerCell player)
                return player.Id == Id &&
                       player.GetType() == GetType();

            return false;
        }

        public override int GetHashCode() => base.GetHashCode();

        public virtual void MoveGeneral(Position move)
        {
            var mapCells = Map.Cells.OfType<MapCell>();

            int m = move.X - Position.X + move.Y - Position.Y;
            bool xDir = move.X - Position.X != 0;
            if (MovementOccupied(m, xDir))
                return;

            var cell = mapCells.FirstOrDefault(x => IsOnPosition(Position, x));

            SetPosition(move);

            if (cell != null)
                cell.Invalid = true;
        }

        public void MoveGeneralRelative(Position move)
            => MoveGeneral(Position + move);

        protected bool MovementOccupied(int move, bool XDirection)
        {
            for (int i = move < 0 ? move : 0; i < (move < 0 ? 0 : move); i++)
            {
                if (XDirection)
                {
                    if (Map.IsTileOccupied(new Position(Position.X + i, Position.Y)))
                        return true;
                }
                else
                {
                    if (Map.IsTileOccupied(new Position(Position.X, Position.Y + i)))
                        return true;
                }
            }

            return false;
        }

        public static bool Equals(PlayerCell player1, PlayerCell player2)
        {
            if (player1 is null && player2 is null)
                return true;
            else if (player1 is null ^ player2 is null)
                return false;

            return player1.Equals(player2);

        }
        
        public bool TryCollectItem()
        {
            var item = Map.Cells.FirstOrDefault(
                c => c.Position == Position && c is BaseItemCell);

            if (item == null)
                return false;

            Map.Cells.Remove(item);
            for (int i = 0; i < ItemInventory.Count; i++)
            {
                if (ItemInventory[i] == null)
                {
                    ItemInventory[i] = item as BaseItemCell;
                    break;
                }
            }
            
            return true;
        }

        public static bool operator ==(PlayerCell player1, PlayerCell player2)
            => Equals(player1, player2);

        public static bool operator !=(PlayerCell player1, PlayerCell player2)
            => !Equals(player1, player2);
    }
}

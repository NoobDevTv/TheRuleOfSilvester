using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TheRuleOfSilvester.Core.Cells;
using TheRuleOfSilvester.Core.Interfaces;
using TheRuleOfSilvester.Core.Items;
using TheRuleOfSilvester.Core.Roles;

namespace TheRuleOfSilvester.Core
{
    public class Player : Cell, IByteSerializable
    {
        public string Name
        {
            get => name; set
            {
                if (value.Length > 20)
                    name = value.Substring(0, 20);
                else
                    name = value;
            }
        }
        public char Avatar { get; private set; }
        public bool IsLocal { get; set; }
        public BaseItemCell[] ItemInventory { get; set; }
        public List<Cell> CellInventory { get; set; }
        public int Id { get; set; }

        public BaseRole Role { get; private set; }

        public event EventHandler<Cell> PlayerChangedCell;

        private readonly int moveSizeX;
        private readonly int moveSizeY;

        private bool ghostMode;
        private GhostPlayer ghost;
        private string name;

        public Player() : base(1, 1, null)
        {
            IsLocal = false;
            Color = Color.Yellow;
            Lines = new BaseElement[1, 1];
            Name = "Tim";
            Role = RoleManager.GetRandomRole();
            CellInventory = new List<Cell>();
            ItemInventory = new BaseItemCell[10];
            moveSizeX = 5;
            moveSizeY = 3;
        }
        public Player(Map map, BaseRole role) : base(1, 1, map)
        {
            ItemInventory = new BaseItemCell[10];
            CellInventory = new List<Cell>();
            Lines = new BaseElement[1, 1];
            IsLocal = true;
            moveSizeX = 5;
            moveSizeY = 3;
            Name = "Tim";
            ghostMode = false;
            Role = role;
            var random = new Random();

            GenerateInventory(map, random);

            map.TextCells.Add(new TextCell("Inventory:", map) { Position = new Point(0, (map.Height + 1) * 3 + 1) });

            SetAvatar(role.Avatar);
        }

        public void SetMap(Map map)
        {
            if (Map == null)
                Map = map;
        }
               
        public void SetAvatar(char avatar)
        {
            Avatar = avatar;
            Lines[0, 0] = Avatar;
        }

        public void MoveUp(bool ghostMode)
        {
            if (ghostMode)
            {
                ghost.MoveUp();
                return;
            }

            MoveGeneral(new Point(Position.X, Position.Y - moveSizeY));
        }

        public void MoveDown(bool ghostMode)
        {
            if (ghostMode)
            {
                ghost.MoveDown();
                return;
            }

            MoveGeneral(new Point(Position.X, Position.Y + moveSizeY));
        }

        public void MoveLeft(bool ghostMode)
        {
            if (ghostMode)
            {
                ghost.MoveLeft();
                return;
            }

            MoveGeneral(new Point(Position.X - moveSizeX, Position.Y));
        }

        public void MoveRight(bool ghostMode)
        {
            if (ghostMode)
            {
                ghost.MoveRight();
                return;
            }

            MoveGeneral(new Point(Position.X + moveSizeX, Position.Y));
        }

        public void StartAction()
        {
            //TODO: more functions
            MoveCell();
        }

        public override void Update(Game game)
        {
            var lastInput = game.InputAction;

            if (lastInput == null || !lastInput.Valid)
                return;

            if (ghostMode)
                Invalid = true;

            switch (lastInput.Type)
            {
                case InputActionType.Up:
                    MoveUp(ghostMode);
                    break;
                case InputActionType.Down:
                    MoveDown(ghostMode);
                    break;
                case InputActionType.Left:
                    MoveLeft(ghostMode);
                    break;
                case InputActionType.Right:
                    MoveRight(ghostMode);
                    break;
                case InputActionType.StartAction:
                    StartAction();
                    break;
                default:
                    break;
            }
        }

        private void GenerateInventory(Map map, Random random)
        {
            var cellTypes = map.MapGenerator?.CellTypes;

            for (int i = 0; i < 3; i++)
            {
                var cell = (Cell)Activator.CreateInstance(cellTypes[random.Next(0, cellTypes.Count)], map, true);
                cell.Position = new Point(1 + i * 2, Map.Height + 2);
                CellInventory.Add(cell);
            }
        }

        private void MoveCell()
        {
            if (!ghostMode)
            {
                ghostMode = true;
                ghost = new GhostPlayer(Map, this);

            }
            else
            {
                ghostMode = false;

                var changedCell = ghost.SelectedCell;
                if (!changedCell.Movable)
                {
                    changedCell.Invalid = true;
                    ghost.Dispose();
                    ghost = null;
                    return;
                }
                Map.Cells.Remove(changedCell);

                var inventoryCell = CellInventory.FirstOrDefault();
                CellInventory.Remove(inventoryCell);

                inventoryCell.Position = changedCell.Position;
                inventoryCell.Invalid = true;

                Map.Cells.Add(inventoryCell);

                changedCell.Position = new Point(5, Map.Height + 2);
                changedCell.Invalid = true;
                CellInventory.ForEach(x => { x.Position = new Point(x.Position.X - 2, x.Position.Y); x.Invalid = true; });
                CellInventory.Add(changedCell);


                var cellsToNormalize = Map.Cells.Where(c =>
                        c.Position.X == inventoryCell.Position.X && c.Position.Y == inventoryCell.Position.Y - 1
                    || c.Position.X == inventoryCell.Position.X && c.Position.Y == inventoryCell.Position.Y + 1
                    || c.Position.X == inventoryCell.Position.X - 1 && c.Position.Y == inventoryCell.Position.Y
                    || c.Position.X == inventoryCell.Position.X + 1 && c.Position.Y == inventoryCell.Position.Y)
                    .Select(x => (MapCell)x).ToList();
                cellsToNormalize.ForEach(x => x.NormalizeLayering());

                (inventoryCell as MapCell).NormalizeLayering();
                (changedCell as MapCell).NormalizeLayering();

                ghost.Dispose();
                ghost = null;

                PlayerChangedCell?.Invoke(this, inventoryCell);
            }
        }

        private bool MovementOccupied(int move, bool XDirection)
        {
            for (int i = move < 0 ? move : 0; i < (move < 0 ? 0 : move); i++)
            {
                if (XDirection)
                {
                    if (Map.IsTileOccupied(new Point(Position.X + i, Position.Y)))
                        return true;
                }
                else
                {
                    if (Map.IsTileOccupied(new Point(Position.X, Position.Y + i)))
                        return true;
                }
            }

            return false;
        }

        public void MoveGeneral(Point move)
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
        public void MoveGeneralRelative(Point move)
            => MoveGeneral(Position + new Size(move));

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Avatar);
            binaryWriter.Write(Id);
            binaryWriter.Write(Name);

            Role.Serialize(binaryWriter);

            binaryWriter.Write(Position.X);
            binaryWriter.Write(Position.Y);

            binaryWriter.Write(CellInventory.Count);

            foreach (MapCell cell in CellInventory)
                cell.Serialize(binaryWriter);
        }

        public void Deserialize(BinaryReader binaryReader)
        {
            SetAvatar(binaryReader.ReadChar());
            Id = binaryReader.ReadInt32();
            Name = binaryReader.ReadString();

            Role = (BaseRole)Activator.CreateInstance(Type.GetType(binaryReader.ReadString()));

            Position = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());

            var count = binaryReader.ReadInt32();

            for (int i = 0; i < count; i++)
                CellInventory.Add(SerializeHelper.DeserializeMapCell(binaryReader));

        }

        public bool TryCollectItem()
        {
            var item = Map.Cells.FirstOrDefault(
                c => c.Position == Position && typeof(BaseItemCell).IsAssignableFrom(c.GetType()));

            if (item == null)
                return false;

            Map.Cells.Remove(item);
            for (int i = 0; i < ItemInventory.Length; i++)
            {
                if (ItemInventory[i] == null)
                {
                    ItemInventory[i] = item as BaseItemCell;
                    break;
                }
            }
            
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is Player player)
                return player.Id == Id;

            return false;
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool Equals(Player player1, Player player2)
        {
            if (((object)player1) == null && ((object)player2) == null)
            {
                return true;
            }
            else if (((object)player1) == null && ((object)player2) != null ||
                     ((object)player1) != null && ((object)player2) == null)
            {
                return false;
            }
            else
            {
                return player1.Equals(player2);
            }

        }

        public static bool operator ==(Player player1, Player player2)
            => Equals(player1, player2);

        public static bool operator !=(Player player1, Player player2)
            => !Equals(player1, player2);
    }
}

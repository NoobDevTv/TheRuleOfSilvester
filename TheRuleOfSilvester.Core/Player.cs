using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core
{
    public class Player : Cell, IByteSerializable
    {
        public string Name { get; set; }
        public Char Avatar { get; private set; }
        public bool IsLocal { get; set; }
        public List<Cell> Inventory { get; set; }
        public int Id { get; set; }

        private readonly int moveSizeX;
        private readonly int moveSizeY;

        private bool ghostMode;
        private GhostPlayer ghost;

        public Player() : base(1, 1, null)
        {
            IsLocal = false;
            Color = Color.Yellow;
            Lines = new BaseElement[1, 1];
        }
        public Player(Map map) : base(1, 1, map)
        {
            Inventory = new List<Cell>();
            Lines = new BaseElement[1, 1];
            IsLocal = true;
            moveSizeX = 5;
            moveSizeY = 3;

            ghostMode = false;
            var random = new Random();

            for (int i = 0; i < 3; i++)
            {
                var cellTypes = map.MapGenerator?.CellTypes; //TODO: get inventory from server
                var cell = (Cell)Activator.CreateInstance(cellTypes[random.Next(0, cellTypes.Count)], map, true);
                cell.Position = new Point(1 + i * 2, Map.Height + 2);
                Inventory.Add(cell);
            }

            map.TextCells.Add(new TextCell("Inventory:", map) { Position = new Point(0, (map.Height + 1) * 3 + 1) });
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

        public void MoveUp()
        {
            if (Position.Y - moveSizeY <= 0 || MovementOccupied(-moveSizeY, false))
                return;

            MoveGeneral(new Point(Position.X, Position.Y - moveSizeY));
        }

        public void MoveDown()
        {
            if (Position.Y >= Map.Height * Map.Cells.FirstOrDefault().Height || MovementOccupied(moveSizeY, false))
                return;

            MoveGeneral(new Point(Position.X, Position.Y + moveSizeY));
        }

        public void MoveLeft()
        {
            if (Position.X - moveSizeX <= 0 || MovementOccupied(-moveSizeX, true))
                return;

            MoveGeneral(new Point(Position.X - moveSizeX, Position.Y));
        }

        public void MoveRight()
        {
            if (Position.X == Map.Width * Map.Cells.FirstOrDefault().Width || MovementOccupied(moveSizeX, true))
                return;

            MoveGeneral(new Point(Position.X + moveSizeX, Position.Y));
        }

        public void StartAction()
        {
            //TODO: more functions
            MoveCell();
        }

        public override void Update(Game game)
        {
            var inputComponent = game.InputCompoment;

            if (ghostMode)
                Invalid = true;

            if (inputComponent.Up && !ghostMode)
                MoveUp();
            else if (inputComponent.Up)
                ghost.MoveUp();

            if (inputComponent.Down && !ghostMode)
                MoveDown();
            else if (inputComponent.Down)
                ghost.MoveDown();

            if (inputComponent.Left && !ghostMode)
                MoveLeft();
            else if (inputComponent.Left)
                ghost.MoveLeft();

            if (inputComponent.Right && !ghostMode)
                MoveRight();
            else if (inputComponent.Right)
                ghost.MoveRight();

            if (inputComponent.StartAction)
                StartAction();

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

                var inventoryCell = Inventory.FirstOrDefault();
                Inventory.Remove(inventoryCell);

                inventoryCell.Position = changedCell.Position;
                inventoryCell.Invalid = true;

                Map.Cells.Add(inventoryCell);

                changedCell.Position = new Point(5, Map.Height + 2);
                changedCell.Invalid = true;
                Inventory.ForEach(x => { x.Position = new Point(x.Position.X - 2, x.Position.Y); x.Invalid = true; });
                Inventory.Add(changedCell);


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
            var cell = Map.Cells.FirstOrDefault(x =>
            x.Position.X * x.Width < Position.X && (x.Position.X * x.Width + x.Width) > Position.X
            && x.Position.Y * x.Height < Position.Y && (x.Position.Y * x.Height + x.Height) > Position.Y);
            SetPosition(move);
            if (cell != null)
                cell.Invalid = true;
        }


        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Avatar);
            binaryWriter.Write(Id);
            binaryWriter.Write(Name);
            binaryWriter.Write(Position.X);
            binaryWriter.Write(Position.Y);
        }

        public void Deserialize(BinaryReader binaryReader)
        {
            SetAvatar(binaryReader.ReadChar());
            Id = binaryReader.ReadInt32();
            Name = binaryReader.ReadString();
            Position = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());
        }

        public override bool Equals(object obj)
        {
            if (obj is Player player)
                return player.Id == Id;

            return false;
        }
    }
}

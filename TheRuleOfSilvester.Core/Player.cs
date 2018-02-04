using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core
{
    public class Player : Cell
    {
        public string Name { get; set; }
        public Char Avatar { get; private set; }

        public List<Cell> Inventory { get; set; }

        private int moveSizeX;
        private int moveSizeY;

        private bool ghostMode;
        private GhostPlayer ghost;


        public Player(Map map) : base(1, 1, map)
        {
            Inventory = new List<Cell>();
            Lines = new char[1, 1];
            moveSizeX = 5;
            moveSizeY = 3;

            ghostMode = false;
            var random = new Random();

            for (int i = 0; i < 3; i++)
            {
                var cellTypes = map.MapGenerator.CellTypes;
                var cell = (Cell)Activator.CreateInstance(cellTypes[random.Next(0, cellTypes.Count)], map);
                cell.Position = new Point(1 + i * 2, Map.Height + 2);
                Inventory.Add(cell);
            }

            map.TextCells.Add(new TextCell("Inventory:") { Position = new Point(0, (map.Height + 1) * 3 + 1) });
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

            MoveGeneral(new Point(Position.X , Position.Y + moveSizeY));
        }

        public void MoveLeft()
        {
            if (Position.X - moveSizeX <= 0 || MovementOccupied(-moveSizeX, true))
                return;

            MoveGeneral(new Point(Position.X - moveSizeX, Position.Y ));
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

                Map.Cells.Remove(changedCell);

                var inventoryCell = Inventory.FirstOrDefault();
                Inventory.Remove(inventoryCell);

                inventoryCell.Position = changedCell.Position;
                inventoryCell.Invalid = true;
                Map.Cells.Add(inventoryCell);

                changedCell.Position = new Point(1, Map.Height + 2);
                changedCell.Invalid = true;
                Inventory.Add(changedCell);

                

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

        private void MoveGeneral(Point move)
        {
            var cell = Map.Cells.FirstOrDefault(x =>
            x.Position.X * x.Width < Position.X && (x.Position.X * x.Width + x.Width) > Position.X
            && x.Position.Y * x.Height < Position.Y && (x.Position.Y * x.Height + x.Height) > Position.Y);
            SetPosition(move);
            if (cell != null)
                cell.Invalid = true;
        }

    }
}

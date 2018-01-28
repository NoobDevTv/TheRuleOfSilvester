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
            Lines = new string[1, 1];
            moveSizeX = 3;
            moveSizeY = 5;

            ghostMode = false;
            Inventory.Add(new CornerLeftUp(Map) {
                Position = new Point(Map.Height + 2, 1)
            });
        }

        public void SetAvatar(char avatar)
        {
            Avatar = avatar;
            Lines[0, 0] = Avatar.ToString();
        }

        public void MoveUp()
        {
            if (Position.X - moveSizeX <= 0 || MovementOccupied(-moveSizeX, true))
                return;

            MoveGeneral(new Point(Position.X - moveSizeX, Position.Y));
        }

        public void MoveDown()
        {
            if (Position.X >= Map.Height * Map.Cells.FirstOrDefault().Height || MovementOccupied(moveSizeX, true))
                return;

            MoveGeneral(new Point(Position.X + moveSizeX, Position.Y));
        }

        public void MoveLeft()
        {
            if (Position.Y - moveSizeY <= 0 || MovementOccupied(-moveSizeY, false))
                return;

            MoveGeneral(new Point(Position.X, Position.Y - moveSizeY));
        }

        public void MoveRight()
        {
            if (Position.Y == Map.Width * Map.Cells.FirstOrDefault().Width || MovementOccupied(moveSizeY, false))
                return;

            MoveGeneral(new Point(Position.X, Position.Y + moveSizeY));
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

                changedCell.Position = new Point(Map.Height + 2, 1);
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
            x.Position.X * x.Height < Position.X && (x.Position.X * x.Height + x.Height) > Position.X
            && x.Position.Y * x.Width < Position.Y && (x.Position.Y * x.Width + x.Width) > Position.Y);
            SetPosition(move);
            if (cell != null)
                cell.Invalid = true;
        }

    }
}

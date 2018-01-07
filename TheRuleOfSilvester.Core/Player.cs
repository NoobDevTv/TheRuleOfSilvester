using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public class Player : Cell
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public Char Avatar { get; private set; }

        private int moveSizeX;
        private int moveSizeY;


        public Player(Map map) : base(1, 1, map)
        {
            Lines = new string[1, 1];
            moveSizeX = 3;
            moveSizeY = 5;
        }

        public void SetAvatar(char avatar)
        {
            Avatar = avatar;
            Lines[0, 0] = Avatar.ToString();
        }

        private bool MovementOccupied(int move, bool XDirection)
        {
            for (int i = move < 0 ? move : 0; i < (move < 0 ? 0 : move); i++)
            {
                if (XDirection)
                {
                    if (map.IsTileOccupied(new Point(Position.X + i, Position.Y)))
                        return true;
                }
                else
                {
                    if (map.IsTileOccupied(new Point(Position.X, Position.Y + i)))
                        return true;
                }
            }

            return false;
        }

        private void MoveGeneral(Point move)
        {
            var cell = map.Cells.FirstOrDefault(x =>
            x.Position.X * x.Height < Position.X && (x.Position.X * x.Height + x.Height) > Position.X
            && x.Position.Y * x.Width < Position.Y && (x.Position.Y * x.Width + x.Width) > Position.Y);
            SetPosition(move);
            if (cell != null)
                cell.Invalid = true;
        }

        public void MoveUp()
        {
            if (Position.X - moveSizeX <= 0 || MovementOccupied(-moveSizeX, true))
                return;

            MoveGeneral(new Point(Position.X - moveSizeX, Position.Y));
        }

        public void MoveDown()
        {
            if (Position.X == map.Height || MovementOccupied(moveSizeX, true))
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
            if (Position.Y == map.Width || MovementOccupied(moveSizeY, false))
                return;

            MoveGeneral(new Point(Position.X, Position.Y + moveSizeY));
        }

        public override void Update(Game game)
        {
            var inputComponent = game.InputCompoment;

            if (inputComponent.Up)
                MoveUp();

            if (inputComponent.Down)
                MoveDown();

            if (inputComponent.Left)
                MoveLeft();

            if (inputComponent.Right)
                MoveRight();

        }
    }
}

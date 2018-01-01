using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public class Player : Cell
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public Char Avatar { get; private set; }


        public Player(Map map) : base(1, 1, map)
        {
            Lines = new string[1, 1];
            Lines[0, 0] = "☺";
        }

        public void SetAvatar(char avatar)
        {
            Avatar = avatar;
            Lines[0, 0] = Avatar.ToString();
        }

        public void MoveUp()
        {
            if (Position.X == 0 || map.IsTileOccupied(new Point(Position.X - 3, Position.Y)))
                return;
            SetPosition(new Point(Position.X - 3, Position.Y));
        }

        public void MoveDown()
        {
            if (Position.X == map.Height || map.IsTileOccupied(new Point(Position.X + 3, Position.Y)))
                return;
            SetPosition(new Point(Position.X + 3, Position.Y));
        }

        public void MoveLeft()
        {
            if (Position.Y == 0 || map.IsTileOccupied(new Point(Position.X, Position.Y - 5)))
                return;
            SetPosition(new Point(Position.X, Position.Y - 5));
        }

        public void MoveRight()
        {
            if (Position.Y == map.Width || map.IsTileOccupied(new Point(Position.X, Position.Y + 5)))
                return;
            SetPosition(new Point(Position.X, Position.Y + 5));
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

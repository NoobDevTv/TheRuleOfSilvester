using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    class GhostPlayer : Cell
    {
        public Cell SelectedCell => Map.GetTileAbsolutePos(Position);

        private Player player;

        private int moveSizeX;
        private int moveSizeY;

        public GhostPlayer(Map map, Player original) : base(map)
        {
            Lines = new string[1, 1];
            player = original;
            Color = Color.Green;

            moveSizeX = 3;
            moveSizeY = 5;

            Lines[0, 0] = original.Avatar.ToString();
            Position = player.Position;
            map.Cells.Add(this);
        }

        public void MoveUp()
        {
            if (Position.X - moveSizeX <= 0)
                return;

            MoveGeneral(new Point(Position.X - moveSizeX, Position.Y));
        }

        public void MoveDown()
        {
            if (Position.X >= Map.Height * Map.Cells.FirstOrDefault().Height)
                return;

            MoveGeneral(new Point(Position.X + moveSizeX, Position.Y));
        }

        public void MoveLeft()
        {
            if (Position.Y - moveSizeY <= 0)
                return;

            MoveGeneral(new Point(Position.X, Position.Y - moveSizeY));
        }

        public void MoveRight()
        {
            if (Position.Y >= Map.Width * Map.Cells.FirstOrDefault().Width)
                return;

            MoveGeneral(new Point(Position.X, Position.Y + moveSizeY));
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
        
        public override void Dispose()
        {
            if (disposed)
                return;

            var cell = Map.GetTileAbsolutePos(Position);
            cell.Invalid = true;
            Map.Cells.Remove(this);

            player = null;

            moveSizeX = 0;
            moveSizeY = 0;

            base.Dispose();
        }
    }
}

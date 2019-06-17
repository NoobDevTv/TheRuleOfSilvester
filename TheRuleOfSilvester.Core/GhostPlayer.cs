using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core
{
    class GhostPlayer : PlayerCell
    {
        public Cell SelectedCell => Map.GetTileAbsolutePos(Position);

        private Player player;

        private int moveSizeX;
        private int moveSizeY;
        private TextCell text;

        public GhostPlayer(Map map, Player original) : base(1, 1, map)
        {
            Map = map;
            Lines = new BaseElement[1, 1];
            player = original;
            Color = Color.Green;
            text = new TextCell("Ghostmode ACTIVATED", map) { Position = new Position(0, (Map.Height + 1) * 3)};
          
            text.Color = Color.Red;
            moveSizeX = 5;
            moveSizeY = 3;

            Lines[0, 0] = original.Avatar;
            Position = player.Position;
            map.Players.Add(this);
            map.TextCells.Add(text);
        }

        public void MoveUp()
        {
            if (Position.Y - moveSizeY <= 0)
                return;

            MoveGeneral(new Position(Position.X, Position.Y - moveSizeY));
        }

        public void MoveDown()
        {
            if (Position.Y >= Map.Height * Map.Cells.FirstOrDefault().Height)
                return;

            MoveGeneral(new Position(Position.X, Position.Y + moveSizeY));
        }

        public void MoveLeft()
        {
            if (Position.X - moveSizeX <= 0)
                return;

            MoveGeneral(new Position(Position.X - moveSizeX, Position.Y));
        }

        public void MoveRight()
        {
            if (Position.X >= Map.Width * Map.Cells.FirstOrDefault().Width)
                return;

            MoveGeneral(new Position(Position.X + moveSizeX, Position.Y));
        }

        public override void MoveGeneral(Position move)
        {
            var cell = Map.Cells.FirstOrDefault(x =>
            x.Position.X * x.Width < Position.X && (x.Position.X * x.Width + x.Width) > Position.X
            && x.Position.Y * x.Height < Position.Y && (x.Position.Y * x.Height + x.Height) > Position.Y);
            SetPosition(move);
            if (cell != null)
                cell.Invalid = true;
        }

        public override void Dispose()
        {
            if (disposed)
                return;

            Map.Players.Remove(this);

            var cell = Map.GetTileAbsolutePos(Position);
            cell.Invalid = true;

            text.MakeBlank();
            text.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Invalid" && !text.Invalid)
                    text.Map.TextCells.Remove(text);
                text = null;
            };

            player = null;

            moveSizeX = 0;
            moveSizeY = 0;

            base.Dispose();
        }
    }
}

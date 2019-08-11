using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Position(int both)
        {
            X = both;
            Y = both;
        }   

        public override bool Equals(object obj)
        {
            if (obj is Position pos)
                return X == pos.X && Y == pos.Y;

            return base.Equals(obj);
        }

        public override int GetHashCode()
            => base.GetHashCode(); //Only to prevent warnings

        public override string ToString() 
            => $"{X} / {Y}";

        public static bool operator ==(Position positionA, Position positionB)
        {
            if (positionA is null && positionB is null)
                return true;
            else if (positionA is null ^ positionB is null)
                return false;

            return positionA.Equals(positionB);
        }
        public static bool operator !=(Position positionA, Position positionB)
            => !(positionA == positionB);

        public static Position operator +(Position positionA, Position positionB) 
            => new Position(positionA.X + positionB.X, positionA.Y + positionB.Y);

        public static Position operator +(Position position, Size size)
            => new Position(position.X + size.Width, position.Y + size.Height);

        public static Position operator -(Position positionA, Position positionB) 
            => new Position(positionA.X - positionB.X, positionA.Y - positionB.Y);

        public static Position operator -(Position position, Size size) 
            => new Position(position.X - size.Width, position.Y - size.Height);
    }
}

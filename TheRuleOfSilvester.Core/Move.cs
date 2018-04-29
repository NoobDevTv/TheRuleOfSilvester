using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public enum MoveTypes
    {
        Moved,
        ChangedMapCell,

    }
    public struct Move
    {
        public MoveTypes MoveType;
        public Point Point;

        public Move(MoveTypes moveType, Point point)
        {
            MoveType = moveType;
            Point = point;
        }

        public override string ToString() => MoveType.ToString() + " | " + Point.ToString();
    }
}

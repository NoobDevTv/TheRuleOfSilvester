using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using TheRuleOfSilvester.Core.Interfaces;

namespace TheRuleOfSilvester.Core
{
    public struct PlayerAction : IByteSerializable
    {
        public ActionType ActionType;
        public Point Point;

        public PlayerAction(ActionType moveType, Point point) : this()
        {
            ActionType = moveType;
            Point = point;
        }

        public void Deserialize(BinaryReader binaryReader)
        {
            Point = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());
            ActionType = (ActionType)binaryReader.ReadInt32();
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Point.X);
            binaryWriter.Write(Point.Y);
            binaryWriter.Write((int)ActionType);
        }

        public override string ToString() => ActionType.ToString() + " | " + Point.ToString();
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using TheRuleOfSilvester.Core.Interfaces;

namespace TheRuleOfSilvester.Core
{
    public class PlayerAction : IByteSerializable
    {
        public Player Player { get; set; }

        public uint Order { get; set; }

        public ActionType ActionType { get; private set; }
        public Position Point { get; private set; }

        public PlayerAction()
        {

        }

        public PlayerAction(Player player, ActionType moveType, Position point) : this()
        {
            Player = player;
            ActionType = moveType;
            Point = point;
        }

        public void Deserialize(BinaryReader binaryReader)
        {
            Player = new Player();
            Player.Deserialize(binaryReader);
            Point = new Position(binaryReader.ReadInt32(), binaryReader.ReadInt32());
            ActionType = (ActionType)binaryReader.ReadInt32();
            Order = binaryReader.ReadUInt32();
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            Player.Serialize(binaryWriter);
            binaryWriter.Write(Point.X);
            binaryWriter.Write(Point.Y);
            binaryWriter.Write((int)ActionType);
            binaryWriter.Write(Order);
        }

        public override string ToString() 
            => ActionType.ToString() + " | " + Point.ToString();
    }
}

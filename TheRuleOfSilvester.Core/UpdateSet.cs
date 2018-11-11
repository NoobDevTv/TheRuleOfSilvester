using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TheRuleOfSilvester.Core.Interfaces;

namespace TheRuleOfSilvester.Core
{
    public class UpdateSet : IByteSerializable
    {
        public Player Player { get; set; }
        public List<PlayerAction> PlayerActions { get; set; }

        public UpdateSet()
        {

        }

        public UpdateSet(Player player, List<PlayerAction> value)
        {
            Player = player;
            PlayerActions = value;
        }

        public void Deserialize(BinaryReader binaryReader)
        {
            Player = new Player();
            Player.Deserialize(binaryReader);
            var bytes = binaryReader.ReadBytes(binaryReader.ReadInt32());
            PlayerActions = (List<PlayerAction>)SerializeHelper.DeserializeToList<PlayerAction>(bytes);
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            Player.Serialize(binaryWriter);
            var bytes = SerializeHelper.SerializeList<PlayerAction>(PlayerActions);
            binaryWriter.Write(bytes.Length);
            binaryWriter.Write(bytes);
        }
    }
}

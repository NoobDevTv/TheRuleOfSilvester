using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Network
{
    public class GameSession : IByteSerializable
    {
        public int MaxPlayers { get; set; }
        public string Name { get; set; }
        public int CurrentPlayers { get; set; }

        public GameSession()
        {
            MaxPlayers = 4;
        }


        public void Deserialize(BinaryReader binaryReader)
        {
            MaxPlayers = binaryReader.ReadInt32();
            Name = binaryReader.ReadString();
            CurrentPlayers = binaryReader.ReadInt32();
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(MaxPlayers);
            binaryWriter.Write(Name);
            binaryWriter.Write(CurrentPlayers);
        }

        public override string ToString() => $"{Name} ->  Has {CurrentPlayers} from {MaxPlayers}";
    }
}

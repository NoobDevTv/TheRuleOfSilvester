using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network.Sessions;

namespace TheRuleOfSilvester.Network.Info
{
    public struct GameServerSessionInfo : IByteSerializable
    {
        public int Id { get; set; }
        public int MaxPlayers { get; private set; }
        public string Name { get; private set; }
        public int CurrentPlayers { get; private set; }

        public GameServerSessionInfo(IGameServerSession gameServerSession)
        {
            Id = gameServerSession.Id;
            MaxPlayers = gameServerSession.MaxPlayers;
            Name = gameServerSession.Name;
            CurrentPlayers = gameServerSession.CurrentPlayers;
        }

        public void Deserialize(BinaryReader binaryReader)
        {
            Id = binaryReader.ReadInt32();
            MaxPlayers = binaryReader.ReadInt32();
            Name = binaryReader.ReadString();
            CurrentPlayers = binaryReader.ReadInt32();
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Id);
            binaryWriter.Write(MaxPlayers);
            binaryWriter.Write(Name);
            binaryWriter.Write(CurrentPlayers);
        }

        public override string ToString()
            => $"{Name} has {CurrentPlayers} from {MaxPlayers}";
    }
}

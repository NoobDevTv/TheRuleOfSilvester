using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Server.Commands
{
    public partial class MapCommands
    {
        [Command((short)1)]
        public static byte[] GetMap(byte[] data) => SerializeHelper.ToByteArray(GameManager.Map);

        [Command((short)2)]
        public static byte[] RegisterNewPlayer(byte[] data)
        {
            GameManager.Map.Players.Add(SerializeHelper.Deserialize<Player>(data)); 
            return new byte[] {0, 2 };
        }

        [Command((short)3)]
        public static byte[] GetPlayers(byte[] data) => SerializeHelper.ToByteArray<Player, List<Player>>(GameManager.Map.Players);

        [Command((short)4)]
        public static byte[] UpdatePlayer(byte[] data)
        {
            var newPlayer = SerializeHelper.Deserialize<Player>(data);
            var player = GameManager.Map.Players.FirstOrDefault(p=> newPlayer.Name == p.Name);
            player.Position = newPlayer.Position;
            return new byte[] { 0,2 };
        }

    }
}

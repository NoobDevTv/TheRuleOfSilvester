using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server.Commands
{
    public partial class MapCommands
    {
        [Command(CommandNames.GetMap)]
        public static byte[] GetMap(byte[] data) => SerializeHelper.ToByteArray(GameManager.Map);

        [Command(CommandNames.RegisterNewPlayer)]
        public static byte[] RegisterNewPlayer(byte[] data)
        {
            GameManager.Map.Players.Add(SerializeHelper.Deserialize<Player>(data));
            return new byte[] { 0, 2 };
        }

        [Command(CommandNames.GetPlayers)]
        public static byte[] GetPlayers(byte[] data) => SerializeHelper.ToByteArray<Player, List<Player>>(GameManager.Map.Players);

        [Command(CommandNames.UpdatePlayer)]
        public static byte[] UpdatePlayer(byte[] data)
        {
            var newPlayer = SerializeHelper.Deserialize<Player>(data);
            var player = GameManager.Map.Players.FirstOrDefault(p => newPlayer.Name == p.Name);
            player.Position = newPlayer.Position;
            return new byte[] { 0, 2 };
        }

    }
}

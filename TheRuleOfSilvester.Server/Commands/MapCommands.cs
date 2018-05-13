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
            => BitConverter.GetBytes(GameManager.AddNewPlayer(SerializeHelper.Deserialize<Player>(data)));


        [Command(CommandNames.GetPlayers)]
        public static byte[] GetPlayers(byte[] data) 
            => SerializeHelper.ToByteArray<Player, List<Player>>(GameManager.Map.Players);

        [Command(CommandNames.UpdatePlayer)]
        public static byte[] UpdatePlayer(byte[] data)
        {
            var newPlayer = SerializeHelper.Deserialize<Player>(data);
            var player = GameManager.Map.Players.FirstOrDefault(p => newPlayer.Id == p.Id);
            player.Position = newPlayer.Position;
            return BitConverter.GetBytes((short)CommandNames.UpdatePlayer);
        }

    }
}

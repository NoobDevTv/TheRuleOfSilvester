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
        [Command((short)CommandNames.GetMap)]
        public static byte[] GetMap(CommandArgs args) => SerializeHelper.Serialize(GameManager.Map);
        
        [Command((short)CommandNames.GetPlayers)]
        public static byte[] GetPlayers(CommandArgs args)
            => SerializeHelper.Serialize(GameManager.Map.Players.Where(p => p.Id != args.Client.PlayerId).ToList());

        [Command((short)CommandNames.UpdatePlayer)]
        public static byte[] UpdatePlayer(CommandArgs args)
        {
            //if(!args.HavePlayer)
            //    return BitConverter.GetBytes((short)CommandNames.UpdatePlayer);

            var newPlayer = SerializeHelper.Deserialize<Player>(args.Data);
            args.NetworkPlayer.Player.Position = newPlayer.Position;
            return BitConverter.GetBytes((short)CommandNames.UpdatePlayer);
        }

    }
}

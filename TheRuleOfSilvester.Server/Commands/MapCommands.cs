using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server.Commands
{
    public partial class MapCommands
    {
        [Command((short)CommandName.GetMap)]
        public static byte[] GetMap(CommandArgs args) => SerializeHelper.Serialize(GameManager.Map);
        
        [Command((short)CommandName.GetPlayers)]
        public static byte[] GetPlayers(CommandArgs args)
            => SerializeHelper.SerializeList(GameManager.Map.Players.Where(p => p.Id != args.Client.PlayerId).ToList());

        [Command((short)CommandName.UpdatePlayer)]
        public static byte[] UpdatePlayer(CommandArgs args)
        {
            //if(!args.HavePlayer)
            //    return BitConverter.GetBytes((short)CommandNames.UpdatePlayer);

            var newPlayer = SerializeHelper.Deserialize<Player>(args.Data);
            args.NetworkPlayer.Player.Position = newPlayer.Position;
            return BitConverter.GetBytes((short)CommandName.UpdatePlayer);
        }

    }
}

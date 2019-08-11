using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server.Commands
{
    public static partial class GeneralCommands
    {
        [Command((short)CommandName.NewPlayer)]
        public static byte[] NewPlayer(CommandArgs args)
        {
            var playerName = Encoding.UTF8.GetString(args.Data);

            Console.WriteLine($"{playerName} has a joint game");

            return SerializeHelper.Serialize(GameManager.GetNewPlayer(args.Client, playerName));
        }

        [Command((short)CommandName.GetStatus)]
        public static byte[] GetStatus(CommandArgs args) 
            => new byte[] { (byte)args.NetworkPlayer.CurrentServerStatus };

        [Command((short)CommandName.GetWinners)]
        public static byte[] GetWinners(CommandArgs args)
            => SerializeHelper.SerializeList(GameManager.GetWinners());
    }
}

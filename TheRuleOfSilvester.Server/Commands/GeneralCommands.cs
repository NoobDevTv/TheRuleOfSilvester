using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server.Commands
{
    public static partial class GeneralCommands
    {
        [Command((short)CommandNames.NewPlayer)]
        public static byte[] NewPlayer(CommandArgs args)
            => SerializeHelper.Serialize(GameManager.GetNewPlayer(args.Client));

        [Command((short)CommandNames.GetStatus)]
        public static byte[] GetStatus(CommandArgs args) 
            => new byte[] { (byte)args.NetworkPlayer.CurrentServerStatus };
    }
}

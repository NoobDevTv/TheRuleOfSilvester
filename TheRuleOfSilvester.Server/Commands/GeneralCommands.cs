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
            => SerializeHelper.Serialize(GameManager.GetNewPlayer(Encoding.UTF8.GetString(args.Data), args.Client));
    }
}

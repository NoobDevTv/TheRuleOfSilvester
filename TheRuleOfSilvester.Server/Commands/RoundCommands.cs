using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server.Commands
{
    public partial class RoundCommands
    {
        [Command((short)CommandName.TransmitActions)]
        public static byte[] TransmitActions(CommandArgs args)
        {
            var playerActions = SerializeHelper.DeserializeToList<PlayerAction>(args.Data.ToArray()).ToList();
            GameManager.AddRoundActions(args.NetworkPlayer.Player, playerActions.OrderBy(a => a.Order).ToList());

            return BitConverter.GetBytes((short)CommandName.TransmitActions);
        }

        [Command((short)CommandName.EndRound)]
        public static byte[] EndRound(CommandArgs args)
        {
            GameManager.EndRound(args.NetworkPlayer);
            return BitConverter.GetBytes((short)CommandName.EndRound);
        }

        [Command((short)CommandName.Wait)]
        public static byte[] Wait(CommandArgs args)
        {
            return BitConverter
                 .GetBytes(args.NetworkPlayer.RoundMode == RoundMode.Executing)
                 .Concat(SerializeHelper.SerializeList(args.NetworkPlayer.UpdateSets ?? new List<PlayerAction>()))
                 .ToArray();
        }
    }
}

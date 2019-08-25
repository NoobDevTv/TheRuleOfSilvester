using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server.Commands
{
    public partial class RoundCommands : CommandObserver
    {
        public override object OnNext(CommandNotification value) => value.CommandName switch
        {
            CommandName.TransmitActions => TransmitActions(value.Arguments),
            CommandName.EndRound => EndRound(value.Arguments),
            CommandName.Wait => Wait(value.Arguments),

            _ => default,
        };

        public static short TransmitActions(CommandArgs args)
        {
            var playerActions = SerializeHelper.DeserializeToList<PlayerAction>(args.Data.ToArray()).ToList();
            GameManager.AddRoundActions(args.NetworkPlayer.Player, playerActions.OrderBy(a => a.Order).ToList());

            return (short)CommandName.TransmitActions;
        }

        public static short EndRound(CommandArgs args)
        {
            GameManager.EndRound(args.NetworkPlayer);
            return (short)CommandName.EndRound;
        }

        public static byte[] Wait(CommandArgs args)
        {
            return BitConverter
                 .GetBytes(args.NetworkPlayer.RoundMode == RoundMode.Executing)
                 .Concat(SerializeHelper.SerializeList(args.NetworkPlayer.UpdateSets ?? new List<PlayerAction>()))
                 .ToArray();
        }
    }
}

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
        [Command((short)CommandNames.TransmitActions)]
        public static byte[] TransmitActions(CommandArgs args)
        {
            var playerActions = SerializeHelper.Deserialize<PlayerAction, List<PlayerAction>>(args.Data.ToArray());
            GameManager.AddRoundActions(args.NetworkPlayer.Player, playerActions);

            return BitConverter.GetBytes((short)CommandNames.TransmitActions);
        }

        [Command((short)CommandNames.EndRound)]
        public static byte[] EndRound(CommandArgs args)
        {
            GameManager.EndRound(args.NetworkPlayer);
            return BitConverter.GetBytes((short)CommandNames.EndRound);
        }

        [Command((short)CommandNames.Wait)]
        public static byte[] Wait(CommandArgs args)
        {
            var resetEvent = new ManualResetEvent(true);
            void setResetEvent(object sender, RoundMode roundMode)
            {
                resetEvent.Set();
            }

            args.NetworkPlayer.OnRoundModeChange += setResetEvent;

            if (args.NetworkPlayer.RoundMode != RoundMode.Planning)
                resetEvent.Reset();

            resetEvent.WaitOne();
            args.NetworkPlayer.OnRoundModeChange -= setResetEvent;

            return BitConverter.GetBytes((short)CommandNames.Wait);
        }
    }
}

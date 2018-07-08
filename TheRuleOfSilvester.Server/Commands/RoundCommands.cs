﻿using CommandManagementSystem.Attributes;
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
            var playerActions = SerializeHelper.DeserializeToList<PlayerAction>(args.Data.ToArray()).ToList();
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

            if (args.NetworkPlayer.RoundMode == RoundMode.Executing)
                resetEvent.Set();

            resetEvent.WaitOne();
            args.NetworkPlayer.OnRoundModeChange -= setResetEvent;
            
            return SerializeHelper.Serialize(args.NetworkPlayer.UpdateSets);
        }
    }
}

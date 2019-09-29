using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server.Commands
{
    public sealed class GeneralCommandObserver : CommandObserver
    {
        public override object OnNext(CommandNotification value) => value.CommandName switch
        {
            CommandName.NewPlayer => NewPlayer(value.Arguments),
            CommandName.GetStatus => GetStatus(value.Arguments),
            CommandName.GetWinners => GetWinners(value.Arguments),

            _ => default,
        };

        public static Player NewPlayer(CommandArgs args)
        {
            var playerName = Encoding.UTF8.GetString(args.Data);

            Console.WriteLine($"{playerName} has a joint game");

            return GameManager.GetNewPlayer(args.Client, playerName);
        }

        public static byte GetStatus(CommandArgs args) 
            => (byte)args.NetworkPlayer.CurrentServerStatus;

        public static List<IPlayer> GetWinners(CommandArgs args)
            => GameManager.GetWinners();

    }
}

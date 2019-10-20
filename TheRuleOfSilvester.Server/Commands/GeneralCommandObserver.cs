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
        private readonly GameManager gameManager;
        private readonly PlayerService playerService;

        public GeneralCommandObserver(GameManager gameManager, PlayerService playerService)
        {
            this.gameManager = gameManager;
            this.playerService = playerService;
        }

        public override object OnNext(CommandNotification value) => value.CommandName switch
        {
            CommandName.NewPlayer => NewPlayer(value.Arguments),
            CommandName.GetStatus => GetStatus(value.Arguments),
            CommandName.GetWinners => GetWinners(value.Arguments),

            _ => default,
        };

        public Player NewPlayer(CommandArgs args)
        {
            if (!playerService.TryGetNetworkPlayer(args.Client.Player, out var player))
                return null;
            
            Console.WriteLine($"{player.PlayerName} has a joint game");

            return gameManager.AddPlayer(player);
        }

        public ServerStatus GetStatus(CommandArgs args)
        {
            if (!playerService.TryGetNetworkPlayer(args.Client.Player, out var player))
                return 0;

            return player.CurrentServerStatus;
        }

        public List<IPlayer> GetWinners(CommandArgs args)
            => gameManager.GetWinners();

    }
}

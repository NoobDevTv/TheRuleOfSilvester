using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
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

            TryAddCommand(CommandName.NewPlayer, NewPlayer);
            TryAddCommand(CommandName.GetStatus, GetStatus);
            TryAddCommand(CommandName.GetWinners, GetWinners);
        }


        public void NewPlayer(BaseClient client, Notification notification)
        {
            if (!playerService.TryGetNetworkPlayer(client, out var player))
                return;
            
            Console.WriteLine($"{player.PlayerName} has a joint game");

            var gamePlayer = gameManager.AddPlayer(player);
            Send(client, new Notification(SerializeHelper.Serialize(gamePlayer), NotificationType.Player));
        }

        public void GetStatus(BaseClient client, Notification notification)
        {
            if (!playerService.TryGetNetworkPlayer(client, out var player))
                return;

            Send(client, new Notification(SerializeHelper.SerializeEnum(player.CurrentServerStatus), NotificationType.ServerStatus));
        }

        public void GetWinners(BaseClient client, Notification notification)
        {
            Send(client, new Notification(SerializeHelper.SerializeList(gameManager.GetWinners()), NotificationType.Winner));
        }

        public override IDisposable Register(IObservable<CommandNotification> observable)
        {
           return observable
                .Subscribe(n => TryDispatch(n));
        }
    }
}

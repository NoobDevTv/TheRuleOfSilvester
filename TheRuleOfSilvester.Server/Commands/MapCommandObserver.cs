using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server.Commands
{
    public class MapCommandObserver : CommandObserver
    {
        private readonly GameManager gameManager;
        private readonly PlayerService playerService;

        public MapCommandObserver(GameManager gameManager, PlayerService playerService)
        {
            this.gameManager = gameManager;
            this.playerService = playerService;

            TryAddCommand(CommandName.GetMap, GetMap);
            TryAddCommand(CommandName.GetPlayers, GetPlayers);
            TryAddCommand(CommandName.UpdatePlayer, UpdatePlayer);
        }

        public void GetMap(BaseClient client, Notification notification)
        {
            Send(client, new Notification(SerializeHelper.Serialize(gameManager.Map), NotificationType.Map));
        }

        public void GetPlayers(BaseClient client, Notification notification)
        {
            if (!playerService.TryGetNetworkPlayer(client, out var networkPlayer))
                throw new NotSupportedException();
            
            var players = gameManager.Map.Players.Where(p => p != (PlayerCell)networkPlayer.Player);
            Send(client, new Notification(SerializeHelper.SerializeList(players), NotificationType.Players));
        }

        public void UpdatePlayer(BaseClient client, Notification notification)
        {
            var newPlayer = notification.Deserialize(SerializeHelper.Deserialize<Player>);

            if(!playerService.TryGetNetworkPlayer(client, out var networkPlayer))
                throw new NotSupportedException();

            if (networkPlayer.Player is Cell player)
                player.Position = newPlayer.Position;                

        }

        public override IDisposable Register(IObservable<CommandNotification> observable)
        {
            return observable
                 .Subscribe(n => TryDispatch(n));
        }

    }
}

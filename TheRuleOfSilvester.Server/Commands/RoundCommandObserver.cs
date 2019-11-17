using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server.Commands
{
    public partial class RoundCommandObserver : CommandObserver
    {
        private readonly GameManager gameManager;
        private readonly PlayerService playerService;

        public RoundCommandObserver(GameManager gameManager, PlayerService playerService)
        {
            this.gameManager = gameManager;
            this.playerService = playerService;

            TryAddCommand(CommandName.TransmitActions, TransmitActions);
            TryAddCommand(CommandName.EndRound, EndRound);
            TryAddCommand(CommandName.Wait, Wait);
        }

        public void TransmitActions(BaseClient client, Notification notification)
        {
            if (!playerService.TryGetNetworkPlayer(client, out var networkPlayer))
                throw new NotSupportedException();

            var playerActions = notification.Deserialize(SerializeHelper.DeserializeToList<PlayerAction>);
            gameManager.AddRoundActions(networkPlayer.Player, playerActions.OrderBy(a => a.Order).ToList());
        }

        public void EndRound(BaseClient client, Notification notification)
        {
            if (!playerService.TryGetNetworkPlayer(client, out var networkPlayer))
                throw new NotSupportedException();

            gameManager.EndRound(networkPlayer);
        }

        public void Wait(BaseClient client, Notification notification)
        {
            if (!playerService.TryGetNetworkPlayer(client, out var networkPlayer))
                throw new NotSupportedException();

            var array = BitConverter
                 .GetBytes(networkPlayer.RoundMode == RoundMode.Executing)
                 .Concat(SerializeHelper.SerializeList(networkPlayer.UpdateSets ?? new List<PlayerAction>()))
                 .ToArray();

            Send(client, new Notification(array, NotificationType.PlayerActions));
        }

        public override IDisposable Register(IObservable<CommandNotification> observable)
        {
            return observable
                 .Subscribe(n => TryDispatch(n));
        }
    }
}

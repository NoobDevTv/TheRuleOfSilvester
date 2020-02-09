using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Network.Sessions;
using TheRuleOfSilvester.Runtime;
using TheRuleOfSilvester.Server.Commands;

namespace TheRuleOfSilvester.Server
{
    public sealed class GameServerSession : ServerSession, IGameServerSession
    {
        public int MaxPlayers { get; }
        public string Name { get; }
        public int CurrentPlayers => gameManager.Players.Count;

        private readonly GameManager gameManager;
        private readonly PlayerService playerService;

        public GameServerSession(PlayerService playerService, string name, int maxPlayers) : base()
        {
            gameManager = new GameManager();
            this.playerService = playerService;
            Name = name;
            MaxPlayers = maxPlayers;
        }

        protected override IDisposable RegisterCommands(IObservable<CommandNotification> commands, 
            out IObservable<CommandNotification> notifications)
        {
            var disposables = new CompositeDisposable
            {
                RegisterCommand<GeneralCommandObserver>(commands, out var generalNotifications, gameManager, playerService),
                RegisterCommand<MapCommandObserver>(commands, out var mapNotifications, gameManager, playerService),
                RegisterCommand<RoundCommandObserver>(commands, out var roundNotifications, gameManager, playerService)
            };

            notifications = Observable.Merge(generalNotifications, mapNotifications, roundNotifications);

            return disposables;
        }

        protected override void OnClientAdded(BaseClient client)
        {
            var status = ServerStatus.Waiting;

            if (playerService.TryGetNetworkPlayer(client, out var networkPlayer))
                status = networkPlayer.CurrentServerStatus;


            Send(client, CommandName.GetStatus, new Notification(status.GetBytes(), NotificationType.ServerStatus));
            base.OnClientAdded(client);
        }
    }
}

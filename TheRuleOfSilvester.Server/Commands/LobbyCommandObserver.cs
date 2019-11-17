using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Network.Info;
using TheRuleOfSilvester.Network.Sessions;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server.Commands
{
    public sealed class LobbyCommandObserver : CommandObserver
    {
        private readonly SessionProvider sessionProvider;
        private readonly PlayerService playerService;

        public LobbyCommandObserver(SessionProvider sessionProvider, PlayerService playerService)
        {
            this.sessionProvider = sessionProvider;
            this.playerService = playerService;

            TryAddCommand(CommandName.RegisterPlayer, RegisterPlayer);
            TryAddCommand(CommandName.GetSessions, GetSessions);
            TryAddCommand(CommandName.JoinSession, JoinSession);
            TryAddCommand(CommandName.NewSession, NewSession);
        }

        private void NewSession(BaseClient client, Notification notification)
        {
            var session = new GameServerSession(playerService);
            sessionProvider.Add(session);
            Send(client, new Notification(SerializeHelper.Serialize(new GameServerSessionInfo(session)), NotificationType.Session));
        }

        private void RegisterPlayer(BaseClient client, Notification notification)
        {
            var playerName = notification.Deserialize(Encoding.UTF8.GetString);
            Console.WriteLine($"{playerName} has a joint in the Lobby");
            
           playerService.TryAddPlayer(client, playerName);
        }

        private void GetSessions(BaseClient client, Notification notification)
        {
            var list = sessionProvider
                .OfType<IGameServerSession>()
                .Select(s => new GameServerSessionInfo(s));

            Send(client, new Notification(SerializeHelper.SerializeList(list), NotificationType.Sessions));
        }

        private void JoinSession(BaseClient client, Notification notification)
        {
            var sessionId = notification.Deserialize(b => BitConverter.ToInt32(b, 0));

            if (!sessionProvider.Contains(sessionId))
                return;

            sessionProvider.EnqueueSessionChange(sessionId, client);

        }

        public override IDisposable Register(IObservable<CommandNotification> observable)
        {
            return observable
                 .Subscribe(n => TryDispatch(n));
        }
    }
}

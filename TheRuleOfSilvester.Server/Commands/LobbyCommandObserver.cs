using System;
using System.Collections.Generic;
using System.IO;
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
            string name;
            int maxPlayers;

            using (var stream = new MemoryStream(notification.Deserialize(b => b)))           
            using (var binaryReader = new BinaryReader(stream))
            {
                name = binaryReader.ReadString();
                maxPlayers = binaryReader.ReadInt32();
            }

            var session = new GameServerSession(playerService, name, maxPlayers);
            sessionProvider.Add(session);
            Send(client, new Notification(SerializeHelper.Serialize(new GameServerSessionInfo(session)), NotificationType.Session));
        }

        private void RegisterPlayer(BaseClient client, Notification notification)
        {
            var playerName = notification.Deserialize(Encoding.UTF8.GetString);
            Console.WriteLine($"{playerName} has a joint in the Lobby");
            
           playerService.TryAddPlayer(client, playerName);
        }

        class FakeSession : IGameServerSession
        {
            public FakeSession(int maxPlayers, string name, int currentPlayers, int id)
            {
                MaxPlayers = maxPlayers;
                Name = name;
                CurrentPlayers = currentPlayers;
                Id = id;
            }

            public int MaxPlayers { get; }

            public string Name { get; }

            public int CurrentPlayers { get; }

            public int Id { get; set; }

            public void AddClient(BaseClient client) => throw new NotImplementedException();
            public void RemoveClient(BaseClient client) => throw new NotImplementedException();
        }

        private void GetSessions(BaseClient client, Notification notification)
        {
            var list = sessionProvider
                .OfType<IGameServerSession>()
                .Select(s => new GameServerSessionInfo(s));

            list = new[]
            {
                new GameServerSessionInfo(new FakeSession(10, "Test1", 5, 1)),
                new GameServerSessionInfo(new FakeSession(6, "Name", 3, 19)),
                new GameServerSessionInfo(new FakeSession(2, "Tolles Game", 1, 2)),
                new GameServerSessionInfo(new FakeSession(50, "Nicht Joinen, nur test", 40, 5)),
            };

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

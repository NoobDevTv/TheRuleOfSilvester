using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Network.Info;
using TheRuleOfSilvester.Network.Sessions;

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
        }

        public override object OnNext(CommandNotification value) => value.CommandName switch
        {
            CommandName.RegisterPlayer => RegisterPlayer(value.Arguments),
            CommandName.GetSessions => GetSessions(value.Arguments),
            CommandName.JoinSession => JoinSession(value.Arguments),
            CommandName.NewGame => NewGame(value.Arguments),
            _ => default,
        };

        private object NewGame(CommandArgs arguments)
        {
            var session = new GameServerSession(playerService);
            sessionProvider.Add(session);
            return new GameServerSessionInfo(session);
        }

        private object RegisterPlayer(CommandArgs arguments)
        {
            var playerName = Encoding.UTF8.GetString(arguments.Data);
            Console.WriteLine($"{playerName} has a joint in the Lobby");
            
            return playerService.TryAddPlayer(arguments.Client, playerName);
        }

        private object GetSessions(CommandArgs arguments)
        {
            return sessionProvider
                .OfType<IGameServerSession>()
                .Select(s => new GameServerSessionInfo(s));
        }

        private bool JoinSession(CommandArgs arguments)
        {
            var sessionId = BitConverter.ToInt32(arguments.Data);

            if (!sessionProvider.Contains(sessionId))
                return false;

            sessionProvider.EnqueueSessionChange(sessionId, arguments.Client, ServerSession);

            return true;
        }
    }
}

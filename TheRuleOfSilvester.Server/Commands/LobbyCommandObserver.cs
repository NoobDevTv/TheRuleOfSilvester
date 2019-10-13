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

        public LobbyCommandObserver(SessionProvider sessionProvider)
        {
            this.sessionProvider = sessionProvider;
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
            sessionProvider.Add(new GameServerSession());
            return true;
        }

        private object RegisterPlayer(CommandArgs arguments)
        {
            throw new NotImplementedException();
        }

        private object GetSessions(CommandArgs arguments)
        {
            return sessionProvider
                .OfType<IGameServerSession>()
                .Select(s => new GameServerSessionInfo(s));
        }

        private bool JoinSession(CommandArgs arguments)
        {
            var sessionId = Convert.ToInt32(arguments.Data);

            if (!sessionProvider.Contains(sessionId))
                return false;

            sessionProvider.EnqueueSessionChange(sessionId, arguments.Client, ServerSession);

            return true;
        }
    }
}

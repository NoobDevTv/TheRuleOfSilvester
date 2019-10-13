using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Server.Commands;

namespace TheRuleOfSilvester.Server
{
    public sealed class LobbyServerSession : ServerSession
    {
        private SessionProvider sessionProvider;

        public LobbyServerSession(SessionProvider sessionProvider) : base()
        {
            this.sessionProvider = sessionProvider;
        }

        protected override void RegisterCommands()
        {
            RegisterCommand<LobbyCommandObserver>(sessionProvider);
        }
    }
}

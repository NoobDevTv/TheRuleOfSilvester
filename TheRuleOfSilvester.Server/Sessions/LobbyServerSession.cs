using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Server.Commands;

namespace TheRuleOfSilvester.Server
{
    public sealed class LobbyServerSession : ServerSession
    {
        private readonly SessionProvider sessionProvider;
        private readonly PlayerService playerService;

        public LobbyServerSession(SessionProvider sessionProvider, PlayerService playerService) : base()
        {
            this.sessionProvider = sessionProvider;
            this.playerService = playerService;
        }

        protected override void RegisterCommands()
        {
            RegisterCommand<LobbyCommandObserver>(sessionProvider, playerService);
        }
    }
}

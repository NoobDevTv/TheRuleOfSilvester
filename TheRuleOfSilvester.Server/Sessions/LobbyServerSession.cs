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
               
        protected override IDisposable RegisterCommands(IObservable<CommandNotification> commands, 
            out IObservable<CommandNotification> notifications)
        {
            return RegisterCommand<LobbyCommandObserver>(commands, out notifications, sessionProvider, playerService);
        }

    }
}

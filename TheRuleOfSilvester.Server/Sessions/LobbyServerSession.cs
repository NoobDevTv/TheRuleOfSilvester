using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Server.Commands;

namespace TheRuleOfSilvester.Server
{
    public sealed class LobbyServerSession : ServerSession
    {
        protected override void RegisterCommands()
        {
            RegisterCommand<LobbyCommandObserver>();
        }
    }
}

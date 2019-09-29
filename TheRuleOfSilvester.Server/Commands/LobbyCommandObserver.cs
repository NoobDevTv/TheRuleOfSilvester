using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server.Commands
{
    public sealed class LobbyCommandObserver : CommandObserver
    {
        public override object OnNext(CommandNotification value) => value.CommandName switch
        {
            CommandName.RegisterPlayer => RegisterPlayer(value.Arguments),
            CommandName.GetSessions => GetSessions(value.Arguments),
            CommandName.JoinSession => JoinSession(value.Arguments),

            _ => default,
        };

        private object RegisterPlayer(CommandArgs arguments)
        {
            throw new NotImplementedException();
        }

        private object GetSessions(CommandArgs arguments)
        {
            throw new NotImplementedException();
        }

        private object JoinSession(CommandArgs arguments)
            => throw new NotImplementedException();
    }
}

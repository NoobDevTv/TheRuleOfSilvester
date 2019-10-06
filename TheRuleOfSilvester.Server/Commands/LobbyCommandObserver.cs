using System;
using System.Collections.Generic;
using System.Linq;
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
            CommandName.NewGame => NewGame(value.Arguments),
            _ => default,
        };
        private object NewGame(CommandArgs arguments)
        {
            var gameServerSession = new GameServerSession(new GameManager());

            gameServerSession.Session.Name = "TestGame";
            gameServerSession.AddClient(arguments.Client);
            gameServerSession.Session.CurrentPlayers = gameServerSession.ConnectedClients.Count;
            (Observable as LobbyServerSession).GameSessions.Add(gameServerSession);

            return gameServerSession.Session;
        }

        private object RegisterPlayer(CommandArgs arguments)
        {
            throw new NotImplementedException();
        }

        private object GetSessions(CommandArgs arguments)
        {
            return (Observable as LobbyServerSession).GameSessions.Select(x=>x.Session);
        }

        private object JoinSession(CommandArgs arguments)
        {
            return new NotImplementedException();

        }
    }
}

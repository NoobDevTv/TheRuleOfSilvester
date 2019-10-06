using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Server.Commands;

namespace TheRuleOfSilvester.Server
{
    public sealed class LobbyServerSession : ServerSession
    {
        public List<GameServerSession> GameSessions { get; set; }
        public LobbyServerSession(GameManager gameManager) : base(gameManager)
        {
            GameSessions = new List<GameServerSession>();
        }

        protected override void RegisterCommands()
        {
            RegisterCommand<LobbyCommandObserver>();
        }
    }
}

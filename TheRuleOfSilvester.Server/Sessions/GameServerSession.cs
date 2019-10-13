using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Network.Sessions;
using TheRuleOfSilvester.Server.Commands;

namespace TheRuleOfSilvester.Server
{
    public sealed class GameServerSession : ServerSession, IGameServerSession
    {
        public int MaxPlayers => throw new NotImplementedException();
        public string Name => throw new NotImplementedException();
        public int CurrentPlayers => throw new NotImplementedException();

        private readonly GameManager gameManager;

        public GameServerSession() : base()
        {
            gameManager = new GameManager();
        }

        protected override void RegisterCommands()
        {
            RegisterCommand<GeneralCommandObserver>(gameManager);
            RegisterCommand<MapCommandObserver>(gameManager);
            RegisterCommand<RoundCommandObserver>(gameManager);        
        }
    }
}

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
        public int MaxPlayers => 4;
        public string Name => "";
        public int CurrentPlayers => gameManager.Players.Count;

        private readonly GameManager gameManager;
        private readonly PlayerService playerService;

        public GameServerSession(PlayerService playerService) : base()
        {
            gameManager = new GameManager();
            this.playerService = playerService;
        }

        protected override void RegisterCommands()
        {
            RegisterCommand<GeneralCommandObserver>(gameManager, playerService);
            RegisterCommand<MapCommandObserver>(gameManager, playerService);
            RegisterCommand<RoundCommandObserver>(gameManager, playerService);        
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Server.Commands;

namespace TheRuleOfSilvester.Server
{
    public sealed class GameServerSession : ServerSession
    {
        public GameSession Session{ get; set; }


        public GameServerSession(GameManager gameManager) : base(gameManager)
        {
            Session = new GameSession();
        }

        protected override void RegisterCommands()
        {
            RegisterCommand<GeneralCommandObserver>();
            RegisterCommand<MapCommandObserver>();
            RegisterCommand<RoundCommandObserver>();        
        }
    }
}

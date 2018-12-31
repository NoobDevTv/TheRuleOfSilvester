using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core.Interfaces;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Core
{
    public class DefaultWaitingComponent : IWaitingComponent
    {

        public void Update(Game game)
        {
            if (game.IsMutliplayer)
            {
                WaitForServer(game);
            }
            else
            {
                return; //TODO: Is there a Singleplayer waiting logic??????
            }

        }

        private void WaitForServer(Game game)
        {
            var status = game.MultiplayerComponent.CurrentServerStatus;

            if (status == ServerStatus.Waiting)
                game.CurrentGameStatus = GameStatus.Waiting;
            else if (status == ServerStatus.Started)
                game.CurrentGameStatus = GameStatus.Running;
        }
    }
}

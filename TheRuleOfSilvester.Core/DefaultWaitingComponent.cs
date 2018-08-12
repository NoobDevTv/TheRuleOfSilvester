using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public class DefaultWaitingComponent : IWaitingComponent
    {

        public void Update(Game game)
        {
            if (game.IsMutliplayer)
            {
                WaitForServer(game.MultiplayerComponent);
            }
            else
            {
                return; //TODO: Is there a Singleplayer waiting logic??????
            }

        }

        private void WaitForServer(IMultiplayerComponent multiplayerComponent)
        {
            var status = multiplayerComponent.CurrentServerStatus;
        }
    }
}

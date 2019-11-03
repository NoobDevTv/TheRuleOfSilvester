using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Core;
using System.Reactive.Linq;

namespace TheRuleOfSilvester.Runtime
{
    public class DefaultWaitingComponent : IWaitingComponent
    {

        public void Update(Game game)
        {
        }

        public IDisposable SubscribeGameStatus(Game game)
        {
            if (game.IsMutliplayer)
            {
               return WaitForServer(game.MultiplayerComponent.CurrentServerStatus)
                        .Subscribe(g => game.CurrentGameStatus = g);
            }
            else
            {
                return null; //TODO: Is there a Singleplayer waiting logic??????
            }
        }

        private IObservable<GameStatus> WaitForServer(IObservable<ServerStatus> serverStatus) => serverStatus
                .Select(s => s switch
                {
                    ServerStatus.Waiting => GameStatus.Waiting,
                    ServerStatus.Started => GameStatus.Running
                });
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheRuleOfSilvester.Core.RoundComponents
{
    class WaitingRoundComponent : IRoundComponent
    {
        public RoundMode Round => RoundMode.Waiting;

        public bool RoundEnd { get; private set; }

        public void Start(Game game)
        {
            game.InputCompoment.Active = false;
        }

        public void Stop(Game game)
        {
            //game.MultiplayerComponent?.EndRound(game.Map.Players.First(p => p.IsLocal));
        }

        public void Update(Game game)
        {
            var updateSet = game.MultiplayerComponent?.WaitingForServer();
            RoundEnd = true;
        }
    }
}

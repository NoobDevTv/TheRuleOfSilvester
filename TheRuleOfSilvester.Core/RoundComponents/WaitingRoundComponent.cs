using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheRuleOfSilvester.Core.RoundComponents
{
    class WaitingRoundComponent : IRoundComponent
    {
        public RoundMode Round => RoundMode.Waiting;

        public void Start(Game game)
        {
        }

        public void Stop(Game game)
        {
            game.MultiplayerComponent?.EndRound(game.Map.Players.First(p => p.IsLocal));
        }

        public void Update(Game game)
        {
        }
    }
}

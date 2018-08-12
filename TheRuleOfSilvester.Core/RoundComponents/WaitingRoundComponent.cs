using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheRuleOfSilvester.Core.RoundComponents
{
    class WaitingRoundComponent : IRoundComponent
    {
        public RoundMode Round => RoundMode.Waiting;

        public bool RoundEnd { get; set; }

        public void Start(Game game)
        {
            game.InputCompoment.Active = false;
        }

        public void Stop(Game game)
        {
            game.MultiplayerComponent?.EndRound();
        }

        public void Update(Game game)
        {
            if (game.MultiplayerComponent == null)
            {
                RoundEnd = true;
                return;
            }

            if (game.MultiplayerComponent.GetUpdateSet(out ICollection<UpdateSet> updateSet))
            {
                game.CurrentUpdateSets = updateSet;
                RoundEnd = true;
            };
        }
    }
}

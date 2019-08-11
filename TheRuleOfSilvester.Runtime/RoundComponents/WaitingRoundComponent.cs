using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.RoundComponents
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
            //game.MultiplayerComponent?.EndRound();
        }

        public void Update(Game game)
        {
            if (!game.IsMutliplayer)
            {
                RoundEnd = true;
                return;
            }

            if (game.MultiplayerComponent.GetUpdateSet(out ICollection<PlayerAction> updateSet))
            {
                game.CurrentUpdateSets = updateSet.OrderBy(a => a.Order).ToList();
                RoundEnd = true;
            };
        }
    }
}

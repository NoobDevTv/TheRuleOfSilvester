using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public class Referee
    {
        public IEnumerable<Player> GetWinners(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                var conditions = player.Role.Conditions;
                if (conditions.TrueForAll(x => x.Match(player)))
                    yield return player;
            }
        }
    }
}

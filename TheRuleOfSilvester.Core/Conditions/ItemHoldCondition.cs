using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TheRuleOfSilvester.Core.Items;

namespace TheRuleOfSilvester.Core.Conditions
{
    public class ItemHoldCondition : ICondition
    {
        public Type ItemType { get; set; } 

        public bool Match(Player player)
        {
            return player.ItemInventory.Any(i => ItemType.IsAssignableFrom(i?.GetType()));
        }
    }
}

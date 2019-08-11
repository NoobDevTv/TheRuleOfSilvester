using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TheRuleOfSilvester.Runtime.Items;

namespace TheRuleOfSilvester.Runtime.Conditions
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

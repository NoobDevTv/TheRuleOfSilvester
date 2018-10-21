using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core.Items;

namespace TheRuleOfSilvester.Core.Conditions
{
    public class ItemHoldCondition : ICondition
    {
        public BaseItemCell Item { get; set; } 

        public bool Match(Player player)
        {
            return player.ItemInventory.Contains(Item);
        }
    }
}

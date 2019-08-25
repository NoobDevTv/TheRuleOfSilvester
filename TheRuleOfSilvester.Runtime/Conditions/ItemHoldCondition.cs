using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TheRuleOfSilvester.Runtime.Items;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.Conditions
{
    public class ItemHoldCondition : ICondition
    {
        public Type ItemType { get; set; } 

        public bool Match(IPlayer player)
        {
            if(player is PlayerCell cell)
                return cell.ItemInventory.Any(i => ItemType.IsAssignableFrom(i?.GetType()));

            return false;
        }
    }
}

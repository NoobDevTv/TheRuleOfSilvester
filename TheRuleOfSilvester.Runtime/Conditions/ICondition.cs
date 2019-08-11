using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Runtime.Conditions
{
    public interface ICondition
    {
        bool Match(Player player);
    }
}

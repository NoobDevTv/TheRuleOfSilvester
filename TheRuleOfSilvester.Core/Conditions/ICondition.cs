using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Conditions
{
    public interface ICondition
    {
        bool Match(Player player);
    }
}

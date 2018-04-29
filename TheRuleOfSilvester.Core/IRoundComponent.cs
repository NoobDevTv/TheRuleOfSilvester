using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core.RoundComponents;

namespace TheRuleOfSilvester.Core
{
    public interface IRoundManagerComponent : IUpdateable 
    {
        IRoundComponent Round { get;  }
        RoundMode RoundMode { get; }

    }
}

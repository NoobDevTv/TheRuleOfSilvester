using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Runtime.RoundComponents;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface IRoundManagerComponent : IUpdateable 
    {
        IRoundComponent Round { get;  }
        RoundMode RoundMode { get; }

    }
}

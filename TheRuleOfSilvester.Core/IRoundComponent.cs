using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public interface IRoundComponent : IUpdateable 
    {
        uint Round { get;  }
        RoundMode RoundMode { get; }

    }
}

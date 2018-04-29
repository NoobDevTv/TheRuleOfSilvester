using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.RoundComponents
{
    public interface IRoundComponent
    {
        RoundMode Round { get; }
        void Update(Game game);
        void Start(Game game);
        void Stop(Game game);
    }
}

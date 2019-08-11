using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.RoundComponents
{
    public interface IRoundComponent
    {
        RoundMode Round { get; }
        bool RoundEnd { get; set; }
        
        void Update(Game game);
        void Start(Game game);
        void Stop(Game game);
    }
}

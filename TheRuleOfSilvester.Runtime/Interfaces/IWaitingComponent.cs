using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface IWaitingComponent : IUpdateable
    {
        IDisposable SubscribeGameStatus(Game game);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Network.Sessions
{
    public interface IGameServerSession : IServerSession
    {
        int MaxPlayers { get; }
        string Name { get; }
        int CurrentPlayers { get; }
    }
}

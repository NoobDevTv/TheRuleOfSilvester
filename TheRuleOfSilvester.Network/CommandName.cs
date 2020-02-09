using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Network
{
    public enum CommandName : short
    {
        Error = -2, 
        Disconnect = -1,

        GetMap,
        GetMovableTiles,
        NewPlayer,
        GetPlayers,
        UpdatePlayer,
        TransmitActions,
        EndRound,
        Wait,
        GetStatus,
        GetWinners,
        RegisterPlayer,
        GetSessions,
        JoinSession,
        JoinedSession,
        NewSession,
    }
}

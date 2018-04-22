using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Network
{
    public enum CommandNames : short
    {
        GetMap,
        GetMovableTiles,
        RegisterNewPlayer,
        GetPlayers,
        UpdatePlayer,
    }
}

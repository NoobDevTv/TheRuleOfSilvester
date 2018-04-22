using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Core
{
    public interface IMultiplayerComponent : IUpdateable
    {
        Client Client { get; }
        int Port { get; }
        string Host { get; }
        Map Map { get;}

        void Connect();

        void Disconnect();

        Map GetMap();

        void RegisterNewPlayer(Player player);

        List<Player> GetPlayers();
    }
}

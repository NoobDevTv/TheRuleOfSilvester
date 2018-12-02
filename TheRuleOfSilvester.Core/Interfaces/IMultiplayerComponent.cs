using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Core.Interfaces
{
    public interface IMultiplayerComponent : IUpdateable
    {
        Client Client { get; }
        int Port { get; }
        string Host { get; }
        ServerStatus CurrentServerStatus { get; }

        void Connect();

        void Disconnect();

        Map GetMap();

        Player ConnectPlayer();

        List<Player> GetPlayers();

        void TransmitActions(Stack<PlayerAction> actions, Player player);

        void EndRound();

        bool GetUpdateSet(out ICollection<PlayerAction> updateSet);

        ServerStatus GetServerStatus();
    }
}

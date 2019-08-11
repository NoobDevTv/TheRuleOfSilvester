using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface IMultiplayerComponent : IUpdateable, IObserver<Package>
    {
        Client Client { get; }
        int Port { get; }
        string Host { get; }
        ServerStatus CurrentServerStatus { get; }

        void Connect();

        void Disconnect();

        void Wait();

        Map GetMap();

        Player ConnectPlayer(string playername);

        List<Player> GetPlayers();

        List<Player> GetWinners();

        void TransmitActions(Stack<PlayerAction> actions, Player player);

        void EndRound();

        bool GetUpdateSet(out ICollection<PlayerAction> updateSet);

        ServerStatus GetServerStatus();
    }
}

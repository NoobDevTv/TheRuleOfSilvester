using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Network.Info;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface IMultiplayerComponent : IUpdateable, INotificationObserver<Package>
    {
        Client Client { get; }
        int Port { get; }
        string Host { get; }
        ServerStatus CurrentServerStatus { get; }

        void Connect();

        void Disconnect();

        void Wait();

        Map GetMap();

        bool ConnectPlayer(string playername);

        IEnumerable<Player> GetPlayers();

        IEnumerable<Player> GetWinners();

        void TransmitActions(Stack<PlayerAction> actions, Player player);

        void EndRound();

        bool GetUpdateSet(out ICollection<PlayerAction> updateSet);

        ServerStatus GetServerStatus();
        IEnumerable<GameServerSessionInfo> GetGameSessions();
        bool JoinSession(GameServerSessionInfo serverSessionInfo);
        Player GetPlayer();
        GameServerSessionInfo CreateGame();
    }
}

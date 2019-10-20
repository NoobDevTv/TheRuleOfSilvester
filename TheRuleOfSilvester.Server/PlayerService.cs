using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    public sealed class PlayerService
    {
        private readonly ConcurrentDictionary<string, NetworkPlayer> players;

        public PlayerService()
        {
            players = new ConcurrentDictionary<string, NetworkPlayer>();
        }

        internal bool TryAddPlayer(ConnectedClient client, string playerName)
        {
            var networkPlayer = new NetworkPlayer(playerName);

            if (players.ContainsKey(playerName))
                return false;

            if (!players.TryAdd(playerName, networkPlayer))
                return false;

            client.Player = playerName;
            return true;
        }

        internal bool TryGetNetworkPlayer(string player, out NetworkPlayer networkPlayer)
            => players.TryGetValue(player, out networkPlayer);
    }
}

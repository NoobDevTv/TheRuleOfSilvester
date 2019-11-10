using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    public sealed class PlayerService
    {
        private readonly ConcurrentDictionary<BaseClient, NetworkPlayer> players;

        public PlayerService()
        {
            players = new ConcurrentDictionary<BaseClient, NetworkPlayer>();
        }

        internal bool TryAddPlayer(BaseClient client, string playerName)
        {
            var networkPlayer = new NetworkPlayer(playerName);

            if (players.ContainsKey(client))
                return false;

            if (players.Values.Any(x=>x.PlayerName == playerName))
                return false;

            if (!players.TryAdd(client, networkPlayer))
                return false;

            return true;
        }

        internal bool TryGetNetworkPlayer(BaseClient client, out NetworkPlayer networkPlayer)
            => players.TryGetValue(client, out networkPlayer);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    static class GameManager
    {
        public static Map Map { get; private set; }
        public static Dictionary<int, NetworkPlayer> Players { get; private set; }
        private static Dictionary<Player, List<PlayerAction>> actionCache;

        static GameManager()
        {
            Map = GenerateMap();
            Players = new Dictionary<int, NetworkPlayer>();
            actionCache = new Dictionary<Player, List<PlayerAction>>();
        }

        private static Map GenerateMap() => new MapGenerator().Generate(20, 10);

        internal static void AddRoundActions(Player player, List<PlayerAction> playerActions) 
            => actionCache[player] = playerActions;

        internal static int AddNewPlayer(Player player)
        {
            int tmpId = Players.Count + 1;

            while (Players.ContainsKey(tmpId))
                tmpId++;

            Players.Add(tmpId, new NetworkPlayer(player));
            Map.Players.Add(player);
            player.Id = tmpId;
            return tmpId;
        }
        
        internal static void AddClientToPlayer(int id, ConnectedClient client)
        {
            Players[id].Client = client;
            client.PlayerId = id;
        }

        internal static void EndRound(NetworkPlayer player)
        {
            player.RoundMode++;
            CheckAllPlayersAsync();
        }

        private static void CheckAllPlayersAsync()
        {
            Task.Run(() =>
            {
                var tmpPlayers = Players.Values.ToList();
                foreach (var player in tmpPlayers)
                {
                    if (player.RoundMode != RoundMode.Waiting)
                        return;
                }

                ExecuteCache();
            });
        }

        private static void ExecuteCache()
        {
        }
        
    }
}

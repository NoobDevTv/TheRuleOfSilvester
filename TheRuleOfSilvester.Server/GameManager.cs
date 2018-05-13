using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Server
{
    static class GameManager
    {
        public static Map Map { get; private set; }
        private static Dictionary<int, NetworkPlayer> players;
        private static Dictionary<Player, List<PlayerAction>> actionCache;

        static GameManager()
        {
            Map = GenerateMap();
            players = new Dictionary<int, NetworkPlayer>();
            actionCache = new Dictionary<Player, List<PlayerAction>>();
        }

        private static Map GenerateMap() => new MapGenerator().Generate(20, 10);

        internal static void AddRoundActions(int playerID, List<PlayerAction> playerActions)
        {
            if (!players.ContainsKey(playerID))
                return;

            actionCache[players[playerID].Player] = playerActions;
        }

        internal static int AddNewPlayer(Player player)
        {
            int tmpId = players.Count + 1;

            while (players.ContainsKey(tmpId))
                tmpId++;

            players.Add(tmpId, new NetworkPlayer(player));
            Map.Players.Add(player);
            player.Id = tmpId;
            return tmpId;
        }

        internal static void EndRound(int playerId)
        {
            if (!players.ContainsKey(playerId))
                return;

            var player = players[playerId];
        }
    }
}

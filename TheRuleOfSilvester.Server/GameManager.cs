using System;
using System.Collections.Generic;
using System.Drawing;
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
        private static readonly Dictionary<Player, List<PlayerAction>> actionCache;

        static GameManager()
        {
            Map = GenerateMap();
            Players = new Dictionary<int, NetworkPlayer>();
            actionCache = new Dictionary<Player, List<PlayerAction>>();
        }

        internal static void AddRoundActions(Player player, List<PlayerAction> playerActions)
            => actionCache[player] = playerActions;

        internal static Player GetNewPlayer(string character, ConnectedClient client)
        {
            int tmpId = Players.Count + 1;

            while (Players.ContainsKey(tmpId))
                tmpId++;

            var player = new Player(Map, character)
            {
                Name = Guid.NewGuid().ToString(),
                Position = new Point(2, 1)
            };
            Players.Add(tmpId, new NetworkPlayer(player)
            {
                Client = client
            });
            Map.Players.Add(player);
            player.Id = tmpId;
            client.PlayerId = tmpId;
            return player;
        }

        internal static void EndRound(NetworkPlayer player)
        {
            player.RoundMode++;

            if (player.UpdateSets.Count > 0)
                player.UpdateSets.Clear();

            CheckAllPlayersAsync();
        }

        private static Map GenerateMap()
            => new MapGenerator().Generate(20, 10);

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
            var updateSets = new List<UpdateSet>();

            foreach (var cachEntry in actionCache)
            {
                var player = cachEntry.Key;

                foreach (var action in cachEntry.Value)
                {
                    switch (action.ActionType)
                    {
                        case ActionType.Moved:
                            Map.Players.First(p => p == player).Position += new Size(action.Point);
                            break;
                        case ActionType.ChangedMapCell:
                            var cell = Map.Cells.First(c => c.Position == action.Point);
                            Map.Cells.Remove(cell);
                            var inventoryCell = player.Inventory.First();
                            inventoryCell.Position = cell.Position;
                            inventoryCell.Invalid = true;
                            Map.Cells.Add(inventoryCell);
                            player.Inventory.Remove(inventoryCell);
                            player.Inventory.Add(cell);

                            cell.Position = new Point(5, Map.Height + 2);
                            cell.Invalid = true;
                            player.Inventory.ForEach(x =>
                            {
                                x.Position = new Point(x.Position.X - 2, x.Position.Y);
                                x.Invalid = true;
                            });
                            break;
                        case ActionType.None:
                        default:
                            break;
                    }
                }

                updateSets.Add(new UpdateSet(player, cachEntry.Value));
            }

            foreach (var player in actionCache.Keys)
            {
                var networkPlayer = Players[player.Id];

                networkPlayer.UpdateSets = updateSets;
                networkPlayer.RoundMode++;
            }
        }

    }
}

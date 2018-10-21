using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Roles;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    internal static class GameManager
    {
        public static Map Map { get; private set; }
        public static Dictionary<int, NetworkPlayer> Players { get; private set; }
        private static readonly Dictionary<Player, List<PlayerAction>> actionCache;
        private static readonly Queue<BaseRole> roles;
        static GameManager()
        {
            Map = GenerateMap();
            Players = new Dictionary<int, NetworkPlayer>();
            actionCache = new Dictionary<Player, List<PlayerAction>>();
            roles = RoleManager.GetAllRolesRandomized();
        }

        internal static void AddRoundActions(Player player, List<PlayerAction> playerActions)
            => actionCache[player] = playerActions;

        internal static Player GetNewPlayer(ConnectedClient client)
        {
            int tmpId = Players.Count + 1;

            while (Players.ContainsKey(tmpId))
                tmpId++;

            var player = new Player(Map, roles.Dequeue())
            {
                Name = Convert.ToBase64String(Guid.NewGuid().ToByteArray()), //TODO: Temporary workaround
                Position = new Point(2, 1),
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

        internal static void StopGame()
        {

        }

        internal static void StartGame()
        {
            foreach (var player in Players)
                player.Value.CurrentServerStatus = ServerStatus.Started;
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
                CheckWinCondition(tmpPlayers);
            });
        }

        private static void CheckWinCondition(List<NetworkPlayer> players)
        {
            foreach (var player in players)
            {
                var conditions = player.Player.Role.Conditions;

                foreach (var condition in conditions)
                    condition.Match(player.Player); //TODO: Work with return value
            }
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
                            var inventoryCell = player.CellInventory.First();
                            inventoryCell.Position = cell.Position;
                            inventoryCell.Invalid = true;
                            Map.Cells.Add(inventoryCell);
                            player.CellInventory.Remove(inventoryCell);
                            player.CellInventory.Add(cell);

                            cell.Position = new Point(5, Map.Height + 2);
                            cell.Invalid = true;
                            player.CellInventory.ForEach(x =>
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

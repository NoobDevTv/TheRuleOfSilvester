using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Interfaces;
using TheRuleOfSilvester.Core.Roles;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    internal static class GameManager
    {
        public static Map Map { get; private set; }
        public static Dictionary<int, NetworkPlayer> Players { get; private set; }
        private static readonly Queue<BaseRole> roles;
        private static readonly ActionCache actionCache;
        private static readonly Executor executor;

        private static SemaphoreSlim winnersSemaphore;
        
        private static List<Player> listOfWinners;

        static GameManager()
        {
            listOfWinners = new List<Player>();
            winnersSemaphore = new SemaphoreSlim(1, 1);

            actionCache = new ActionCache();
            executor = new Executor(actionCache);
            Players = new Dictionary<int, NetworkPlayer>();
            roles = RoleManager.GetAllRolesRandomized();
        }

        internal static void GenerateMap(int x, int y) 
            => Map = new MapGenerator().Generate(x, y);

        internal static List<Player> GetWinners()
        {
            var tmpPlayer = new List<Player>();
            winnersSemaphore.Wait();
            tmpPlayer.AddRange(listOfWinners);
            winnersSemaphore.Release();
            return tmpPlayer;
        }

        internal static void AddRoundActions(Player player, List<PlayerAction> playerActions)
        {
            playerActions.ForEach(a => a.Player = player);
            actionCache.AddRange(playerActions);
        }

        internal static Player GetNewPlayer(ConnectedClient client, string playername)
        {
            int tmpId = Players.Count + 1;

            while (Players.ContainsKey(tmpId))
                tmpId++;

            var player = new Player(Map, roles.Dequeue())
            {
                Name = playername, 
                Position = new Position(7, 4),
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
        
        private static void CheckAllPlayersAsync()
        {
            Task.Run(() =>
            {
                var referee = new Referee();
                var tmpPlayers = Players.Values.ToList();
                foreach (var player in tmpPlayers)
                {
                    if (player.RoundMode != RoundMode.Waiting)
                        return;
                }

                Execute();
                var winners = referee.GetWinners(tmpPlayers.Select(t => t.Player));
                if (winners.Count() > 0)
                {
                    Console.Clear();
                    foreach (var winner in winners)
                    {
                        Console.WriteLine($"{winner.Name} has won the match :)");
                    }

                    tmpPlayers.ForEach(p => p.CurrentServerStatus = ServerStatus.Ended);

                    winnersSemaphore.Wait();
                    listOfWinners.AddRange(winners);
                    winnersSemaphore.Release();
                }
            });
        }
               
        private static void Execute()
        {
            executor.Execute(Map);

            foreach (var player in Players.Values)
            {
                player.UpdateSets = executor.UpdateSets;
                player.RoundMode++;
            }
        }

    }
}

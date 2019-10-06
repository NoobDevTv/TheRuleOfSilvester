using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;
using TheRuleOfSilvester.Runtime.Roles;

namespace TheRuleOfSilvester.Server
{
    public class GameManager
    {
        public Map Map { get; private set; }
        public Dictionary<int, NetworkPlayer> Players { get; private set; }
        private readonly Queue<BaseRole> roles;
        private ActionCache actionCache;
        private Executor executor;

        private readonly SemaphoreExtended winnersSemaphore;
        
        private readonly List<IPlayer> listOfWinners;

        public GameManager()
        {
            listOfWinners = new List<IPlayer>();
            winnersSemaphore = new SemaphoreExtended(1, 1);

            actionCache = new ActionCache();
            executor = new Executor(actionCache);
            Players = new Dictionary<int, NetworkPlayer>();
            roles = new Queue<BaseRole>();// RoleManager.GetAllRolesRandomized();
        }

        internal void GenerateMap(int x, int y) 
            => Map = new MapGenerator().Generate(x, y);

        internal List<IPlayer> GetWinners()
        {
            var tmpPlayer = new List<IPlayer>();
            using (winnersSemaphore.Wait())
                tmpPlayer.AddRange(listOfWinners);

            return tmpPlayer;
        }

        internal void AddRoundActions(IPlayer player, List<PlayerAction> playerActions)
        {
            playerActions.ForEach(a => a.Player = player);
            actionCache.AddRange(playerActions);
        }

        internal Player GetNewPlayer(ConnectedClient client, string playername)
        {
            int tmpId = Players.Count + 1;

            while (Players.ContainsKey(tmpId))
                tmpId++;

            var player = new Player(Map, roles.Dequeue())
            {
                Name = playername, 
                Position = new Position(7, 4),
            };
            Players.Add(tmpId, new NetworkPlayer(player));
            Map.Players.Add(player);
            player.Id = tmpId;
            client.PlayerId = tmpId;
            return player;
        }

        internal void StopGame()
        {
            listOfWinners.Clear();
            Players?.Clear();
            roles?.Clear();
            foreach (var actions in actionCache)
            {
                actions.Clear();
            }
        }
    

        internal void StartGame()
        {
            foreach (var role in RoleManager.GetAllRolesRandomized())
                roles.Enqueue(role);

            foreach (var player in Players)
                player.Value.CurrentServerStatus = ServerStatus.Started;
        }

        internal void EndRound(NetworkPlayer player)
        {
            player.RoundMode++;

            if (player.UpdateSets.Count > 0)
                player.UpdateSets.Clear();

            CheckAllPlayersAsync();
        }
        
        private void CheckAllPlayersAsync()
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

                    using (winnersSemaphore.Wait())
                        listOfWinners.AddRange(winners);
                }
            });
        }
               
        private void Execute()
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

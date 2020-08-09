using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Extensions;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;
using TheRuleOfSilvester.Runtime.Roles;

namespace TheRuleOfSilvester.Server
{
    public class GameManager
    {
        public Map Map { get; private set; }
        public HashSet<NetworkPlayer> Players { get; private set; }

        private int NextId => nextId++;
        private readonly Queue<BaseRole> roles;
        private readonly ActionCache actionCache;
        private readonly Executor executor;

        private readonly SemaphoreExtended winnersSemaphore;
        private readonly List<IPlayer> listOfWinners;
        private readonly int maxPlayers;
        private int nextId;

        public GameManager(int maxPlayers)
        {
            this.maxPlayers = maxPlayers;
            nextId = 0;
            listOfWinners = new List<IPlayer>();
            winnersSemaphore = new SemaphoreExtended(1, 1);

            actionCache = new ActionCache();
            executor = new Executor(actionCache);
            Players = new HashSet<NetworkPlayer>();
            roles = RoleManager.GetAllRolesRandomized();
            Map = new MapGenerator().Generate(20, 10);
        }
        
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

        internal Player AddPlayer(NetworkPlayer networkPlayer)
        {
            if(Players.Contains(networkPlayer))
            {
                //TODO: throw exception
            }

            var player = new Player(Map, roles.Dequeue())
            {
                Name = networkPlayer.PlayerName, 
                Position = new Position(7, 4),
            };
            Players.Add(networkPlayer);
            Map.Players.Add(player);
            player.Id = NextId;
            networkPlayer.Player = player;

            if (Players.Count == maxPlayers)
                StartGame();

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
                player.CurrentServerStatus = ServerStatus.Started;
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

                foreach (var player in Players)
                {
                    if (player.RoundMode != RoundMode.Waiting)
                        return;
                }

                Execute();
                var winners = referee.GetWinners(Players.Select(t => t.Player));
                if (winners.Count() > 0)
                {
                    Console.Clear();
                    foreach (var winner in winners)
                    {
                        Console.WriteLine($"{winner.Name} has won the match :)");
                    }

                    Players.ForEach(p => p.CurrentServerStatus = ServerStatus.Ended);

                    using (winnersSemaphore.Wait())
                        listOfWinners.AddRange(winners);
                }
            });
        }
               
        private void Execute()
        {
            executor.Execute(Map);

            foreach (var player in Players)
            {
                player.UpdateSets = executor.UpdateSets;
                player.RoundMode++;
            }
        }
    }
}

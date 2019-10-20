using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Runtime;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network.Info;
using TheRuleOfSilvester.Core.Extensions;

namespace TheRuleOfSilvester
{
    public class MultiplayerComponent : IMultiplayerComponent
    {
        public Client Client { get; private set; }

        public int Port { get; set; }
        public string Host { get; set; }
        public ServerStatus CurrentServerStatus { get; set; }

        private IDisposable subscription;
        private readonly Dictionary<int, Awaiter> waitingDic;

        public MultiplayerComponent()
        {
            waitingDic = new Dictionary<int, Awaiter>();
            Client = new Client();
            subscription = Client.Subscribe(this);
        }

        public void Connect()
            => Client.Connect(Host, Port);

        public void Disconnect()
            => Client.Disconnect();

        public void Wait()
          => Client.Wait();

        public void Update(Game game)
        {
            //TODO: Implement waiting screen
            CurrentServerStatus = GetServerStatus();

            switch (CurrentServerStatus)
            {
                case ServerStatus.Started:
                    game.CurrentGameStatus = GameStatus.Running;
                    var players = GetPlayers();
                    players.ForEach(x => x.Map = game.Map);
                    game.Map.Players.AddRange(players);
                    break;
                default:
                    break;
            }

        }

        public ServerStatus GetServerStatus()
        {
            if (TryAwaitableSend(CommandName.GetStatus, out var data))
                return (ServerStatus)data[0];

            return ServerStatus.Error;
        }

        public Map GetMap()
        {
            if (TryAwaitableSend(CommandName.GetMap, out var data))
            {
                return SerializeHelper.Deserialize<Map>(data);
            }

            return null;
        }

        public GameServerSessionInfo CreateGame()
        {
            if (TryAwaitableSend(CommandName.NewGame, out var data))
                return SerializeHelper.Deserialize<GameServerSessionInfo>(data);

            return new GameServerSessionInfo();
        }

        public IEnumerable<GameServerSessionInfo> GetGameSessions()
        {
            if (TryAwaitableSend(CommandName.GetSessions, out var data))
                return SerializeHelper.DeserializeToList<GameServerSessionInfo>(data);

            return Enumerable.Empty<GameServerSessionInfo>();
        }

        public bool JoinSession(GameServerSessionInfo serverSessionInfo)
        {
            if (TryAwaitableSend(CommandName.JoinSession, out var data, BitConverter.GetBytes(serverSessionInfo.Id)))
                return BitConverter.ToBoolean(data);

            return false;
        }

        public bool ConnectPlayer(string playername)
        {
            if (TryAwaitableSend(CommandName.RegisterPlayer, out var data, Encoding.UTF8.GetBytes(playername)))
                return BitConverter.ToBoolean(data);

            return false;
        }

        public IEnumerable<Player> GetPlayers()
        {
            if (TryAwaitableSend(CommandName.GetPlayers, out var data))
                return SerializeHelper.DeserializeToList<Player>(data);

            return Enumerable.Empty<Player>();
        }

        public IEnumerable<Player> GetWinners()
        {
            if (TryAwaitableSend(CommandName.GetWinners, out var data))
                return SerializeHelper.DeserializeToList<Player>(data);

            return Enumerable.Empty<Player>();
        }

        public void UpdatePlayer(Player player)
            => Send(CommandName.UpdatePlayer, SerializeHelper.Serialize(player));

        public Player GetPlayer()
        {
            if (TryAwaitableSend(CommandName.NewPlayer, out var data))
                return SerializeHelper.Deserialize<Player>(data);

            return null;
        }

        public void TransmitActions(Stack<PlayerAction> actions, Player player)
            => Send(CommandName.TransmitActions,
                SerializeHelper.SerializeList(actions.ToList()));

        public void EndRound()
            => Send(CommandName.EndRound);

        public bool GetUpdateSet(out ICollection<PlayerAction> updateSet)
        {
            if (TryAwaitableSend(CommandName.Wait, out var data))
            {
                updateSet = SerializeHelper.DeserializeToList<PlayerAction>(data.Skip(sizeof(bool)).ToArray());
                return BitConverter.ToBoolean(data, 0);
            }
            updateSet = new List<PlayerAction>();
            return false;
        }

        public object OnNext(Package package)
        {
            if (waitingDic.TryGetValue(package.Id, out Awaiter awaiter))
            {
                awaiter.SetResult(package.Data, package.Command >= 0);
                waitingDic.Remove(package.Id);
                return awaiter;
            }
            else
            {
                throw new KeyNotFoundException($"Not found any awaiter with id {package.Id} as Command {package.CommandName}");
            }
        }

        public void OnCompleted()
        {
            subscription?.Dispose();
            subscription = null;
        }

        public void OnError(Exception error) => throw error;

        private Awaiter Send(CommandName command, params byte[] data)
        {
            var package = new Package(command, data);
            var awaiter = new Awaiter(package.Id);

            if (waitingDic.TryAdd(package.Id, awaiter))
                Client.Send(package);
            else
                throw new Exception("Not sendable");

            return awaiter;
        }

        private bool TryAwaitableSend(CommandName command, out byte[] result, params byte[] data)
        {
            var awaiter = Send(command, data);
            awaiter.WaitOn();
            result = awaiter.Data;
            return awaiter.Successfull;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Interfaces;
using TheRuleOfSilvester.Network;

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
            => (ServerStatus)AwaitableSend(CommandName.GetStatus)[0];

        public Map GetMap()
            => SerializeHelper.Deserialize<Map>(AwaitableSend(CommandName.GetMap));

        public Player ConnectPlayer(string playername)
            => SerializeHelper.Deserialize<Player>(AwaitableSend(CommandName.NewPlayer, Encoding.UTF8.GetBytes(playername)));

        public List<Player> GetPlayers()
            => SerializeHelper.DeserializeToList<Player>(AwaitableSend(CommandName.GetPlayers)).ToList();

        public List<Player> GetWinners()
            => SerializeHelper.DeserializeToList<Player>(AwaitableSend(CommandName.GetWinners)).ToList();

        public void UpdatePlayer(Player player)
            => Send(CommandName.UpdatePlayer, SerializeHelper.Serialize(player));

        public void TransmitActions(Stack<PlayerAction> actions, Player player)
            => Send(CommandName.TransmitActions,
                SerializeHelper.SerializeList(actions.ToList()));

        public void EndRound()
            => Send(CommandName.EndRound);

        public bool GetUpdateSet(out ICollection<PlayerAction> updateSet)
        {
            var data = AwaitableSend(CommandName.Wait);
            updateSet = SerializeHelper.DeserializeToList<PlayerAction>(data.Skip(sizeof(bool)).ToArray());
            return BitConverter.ToBoolean(data, 0);
        }

        public void OnNext(Package package)
        {
            if (waitingDic.TryGetValue(package.Id, out Awaiter awaiter))
            {
                awaiter.SetResult(package.Data);
                waitingDic.Remove(package.Id);
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

        private byte[] AwaitableSend(CommandName command, params byte[] data)
        {
            var awaiter = Send(command, data);
            awaiter.WaitOn();
            return awaiter.Data;
        }
    }
}

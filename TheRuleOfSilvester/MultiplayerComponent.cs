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

        public MultiplayerComponent()
            => Client = new Client();

        public void Connect()
            => Client.Connect(Host, Port);

        public void Disconnect()
            => Client.Disconnect();

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
            => (ServerStatus)Send(CommandName.GetStatus)[0];

        public Map GetMap()
            => SerializeHelper.Deserialize<Map>(Send(CommandName.GetMap));

        public Player ConnectPlayer()
            => SerializeHelper.Deserialize<Player>(Send(CommandName.NewPlayer));

        public List<Player> GetPlayers()
            => SerializeHelper.DeserializeToList<Player>(Send(CommandName.GetPlayers)).ToList();

        public void UpdatePlayer(Player player)
            => Send(CommandName.UpdatePlayer, SerializeHelper.Serialize(player));

        public void TransmitActions(Stack<PlayerAction> actions, Player player)
            => Send(CommandName.TransmitActions,
                SerializeHelper.SerializeList(actions.ToList()));

        public void EndRound()
            => Send(CommandName.EndRound);

        public bool GetUpdateSet(out ICollection<PlayerAction> updateSet)
        {
            var answer = Send(CommandName.Wait);

            updateSet = SerializeHelper.DeserializeToList<PlayerAction>(answer.Skip(sizeof(bool)).ToArray());
            return BitConverter.ToBoolean(answer, 0);
        }

        private byte[] Send(CommandName command, params byte[] data)
            => Client.Send(BitConverter.GetBytes((short)command).Concat(data).ToArray());
    }
}

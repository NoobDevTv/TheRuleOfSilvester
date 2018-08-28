using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;
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
            => (ServerStatus)Send(CommandNames.GetStatus)[0];

        public Map GetMap()
            => SerializeHelper.Deserialize<Map>(Send(CommandNames.GetMap));

        public Player ConnectPlayer(string character)
            => SerializeHelper.Deserialize<Player>(Send(CommandNames.NewPlayer, Encoding.UTF8.GetBytes(character)));

        public List<Player> GetPlayers()
            => SerializeHelper.DeserializeToList<Player>(Send(CommandNames.GetPlayers)).ToList();

        public void UpdatePlayer(Player player)
            => Send(CommandNames.UpdatePlayer, SerializeHelper.Serialize(player));

        public void TransmitActions(Stack<PlayerAction> actions, Player player)
            => Send(CommandNames.TransmitActions,
                SerializeHelper.Serialize<PlayerAction>(actions.ToList()));

        public void EndRound()
            => Send(CommandNames.EndRound);

        public bool GetUpdateSet(out ICollection<UpdateSet> updateSet)
        {
            var answer = Send(CommandNames.Wait);

            updateSet = SerializeHelper.DeserializeToList<UpdateSet>(answer.Skip(sizeof(bool)).ToArray());
            return BitConverter.ToBoolean(answer, 0);
        }

        private byte[] Send(CommandNames command, params byte[] data)
            => Client.Send(BitConverter.GetBytes((short)command).Concat(data).ToArray());
    }
}

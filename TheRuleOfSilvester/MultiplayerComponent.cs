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

        public MultiplayerComponent() => Client = new Client();

        public void Connect() => Client.Connect(Host, Port);

        public void Disconnect() => Client.Disconnect();

        public void Update(Game game)
        {
            var tmpList = GetPlayers();
            foreach (var player in tmpList)
            {
                var tmpPlayer = game.Map.Players.FirstOrDefault(p => p.Name == player.Name);

                if (tmpPlayer == null)
                {
                    player.SetMap(game.Map);
                    game.Map.Players.Add(player);
                    player.Invalid = true;
                }
                else if (tmpPlayer.IsLocal)
                {
                    //UpdatePlayer(tmpPlayer);
                    continue;
                }
                else
                {
                    tmpPlayer.MoveGeneral(player.Position);
                    tmpPlayer.Invalid = true;
                }

            }
        }

        public Map GetMap()
            => SerializeHelper.Deserialize<Map>(Send(CommandNames.GetMap));

        public Player Connect(string character)
            => SerializeHelper.Deserialize<Player>(Send(CommandNames.NewPlayer, Encoding.UTF8.GetBytes(character)));

        public List<Player> GetPlayers()
            => SerializeHelper.DeserializeToList<Player>(Send(CommandNames.GetPlayers)).ToList();

        public void UpdatePlayer(Player player)
            => Send(CommandNames.UpdatePlayer, SerializeHelper.Serialize(player));

        public void TransmitActions(Stack<PlayerAction> actions, Player player)
            => Send(CommandNames.TransmitActions,
                SerializeHelper.Serialize<PlayerAction>(actions.ToList()));

        public void EndRound(Player player)
            => Send(CommandNames.EndRound);

        public ICollection<UpdateSet> WaitingForServer()
        {
            var answer = Send(CommandNames.Wait);
            
            return SerializeHelper.DeserializeToList<UpdateSet>(answer);
        }

        private byte[] Send(CommandNames command, params byte[] data)
            => Client.Send(BitConverter.GetBytes((short)command).Concat(data).ToArray());
    }
}

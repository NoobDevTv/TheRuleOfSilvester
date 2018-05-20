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
                    UpdatePlayer(tmpPlayer);
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
            => SerializeHelper.Deserialize<Map>(Client.Send(BitConverter.GetBytes((short)CommandNames.GetMap)));

        public void RegisterNewPlayer(Player player)
            => player.Id = BitConverter.ToInt32(Client.Send(BitConverter
                                                                .GetBytes((short)CommandNames.RegisterNewPlayer)
                                                                .Concat(SerializeHelper.ToByteArray(player))
                                                                .ToArray()), 0);

        public List<Player> GetPlayers()
            => SerializeHelper.Deserialize<Player, List<Player>>(
                Client.Send(BitConverter.GetBytes((short)CommandNames.GetPlayers)));

        public void UpdatePlayer(Player player)
            => Client.Send(BitConverter
                            .GetBytes((short)CommandNames.UpdatePlayer)
                            .Concat(SerializeHelper.ToByteArray(player))
                            .ToArray());

        public void TransmitActions(Stack<PlayerAction> actions, Player player)
            => Client.Send(BitConverter
                            .GetBytes((short)CommandNames.TransmitActions)
                            .Concat(BitConverter.GetBytes(player.Id))
                            .Concat(SerializeHelper.ToByteArray<PlayerAction, List<PlayerAction>>(actions.ToList()))
                            .ToArray());

        public void EndRound(Player player)
            => Client.Send(BitConverter
                            .GetBytes((short)CommandNames.EndRound)
                            .Concat(BitConverter.GetBytes(player.Id))
                            .ToArray());

    }
}

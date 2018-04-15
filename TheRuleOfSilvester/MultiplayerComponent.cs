﻿using System;
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
        public Map Map { get; private set; }

        public MultiplayerComponent() => Client = new Client();

        public void Connect() => Client.Connect(Host, Port);

        public void Disconnect() => Client.Disconnect();

        public void Update(Game game)
        {
            var tmpList = GetPlayers();
            foreach (var player in tmpList)
            {
                var tmpPlayer = Map.Players.FirstOrDefault(p => p.Name == player.Name);

                if (tmpPlayer == null)
                {
                    player.SetMap(Map);
                    Map.Players.Add(player);
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
                    //tmpPlayer.Position = ;
                    
                    tmpPlayer.Invalid = true;
                }

            }
        }

        public Map GetMap()
        {
            Map = SerializeHelper.Deserialize<Map>(Client.Send(new byte[] { 0, 1 }));
            return Map;
        }
        public void RegisterNewPlayer(Player player) => Client.Send(new byte[] { 0, 2 }.Concat(SerializeHelper.ToByteArray(player)).ToArray());

        public List<Player> GetPlayers() => SerializeHelper.Deserialize<Player, List<Player>>(Client.Send(new byte[] { 0, 3 }));

        public void UpdatePlayer(Player player) => Client.Send(new byte[] { 0, 4 }.Concat(SerializeHelper.ToByteArray(player)).ToArray());
    }
}

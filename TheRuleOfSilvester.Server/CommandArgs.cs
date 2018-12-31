using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    public class CommandArgs
    {
        public NetworkPlayer NetworkPlayer { get; set; }
        public byte[] Data { get; set; }
        public ConnectedClient Client { get; }

        public bool HavePlayer => NetworkPlayer != null;

        public CommandArgs(NetworkPlayer player, ConnectedClient client, byte[] data)
        {
            NetworkPlayer = player;
            Data = data;
            Client = client;
        }
    }
}

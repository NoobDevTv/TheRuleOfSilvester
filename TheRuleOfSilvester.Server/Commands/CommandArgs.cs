using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    public class CommandArgs
    {
        public byte[] Data { get; set; }
        public ConnectedClient Client { get; }
        
        public CommandArgs(ConnectedClient client, byte[] data)
        {
            Data = data;
            Client = client;
        }
    }
}

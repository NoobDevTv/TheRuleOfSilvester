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

        }

        public Map GetMap() => SerializeHelper.Deserialize<Map>(Client.Send(new byte[] { 0, 1 }));
    }
}

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Network
{
    public class ConnectedClient : BaseClient
    {
        public string Player { get; set; }

        public ConnectedClient(Socket socket) : base(socket)
        {
        }        
    }
}

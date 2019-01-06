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
        public int PlayerId { get; set; }

        public bool Registered => PlayerId > 0;        

        public ConnectedClient(Socket socket) : base(socket)
        {
            PlayerId = -1;
        }       
    }
}

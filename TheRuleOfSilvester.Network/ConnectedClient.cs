using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Network
{
    class ConnectedClient : BaseClient
    {
        private static int received;

        public ConnectedClient(Socket socket) : base(socket)
        {

        }

        protected override void ProcessInternal(byte[] receiveArgsBuffer, int receiveArgsCount)
        {
            var tmpString = Encoding.UTF8.GetString(receiveArgsBuffer, 0, receiveArgsCount);
            
        }
    }
}

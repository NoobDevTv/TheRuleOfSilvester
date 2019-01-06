﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Network
{
    public class Client : BaseClient
    {        
        public Client() :
            base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
        }
        
        public void Connect(string host, int port)
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault(
                a => a.AddressFamily == Socket.AddressFamily);

            Socket.BeginConnect(new IPEndPoint(address, port), OnConnected, null);
        }        

        private void OnConnected(IAsyncResult ar)
        {
            Socket.EndConnect(ar);
            Start();
        }
    }
}

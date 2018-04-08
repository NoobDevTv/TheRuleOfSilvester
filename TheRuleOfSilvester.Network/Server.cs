using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TheRuleOfSilvester.Network
{
    public class Server : IDisposable
    {
        public event EventHandler<ConnectedClient> OnClientConnected;

        private Socket socket;
        private List<ConnectedClient> connectedClients;
        private readonly object lockObj;

        public Server()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            lockObj = new object();
        }

        public void Start(IPAddress address, int port)
        {
            connectedClients = new List<ConnectedClient>();
            socket.Bind(new IPEndPoint(address, port));
            socket.Listen(1024);
            socket.BeginAccept(OnClientAccepted, null);
        }
        public void Start(string host, int port)
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault(
                a => a.AddressFamily == socket.AddressFamily);

            Start(address, port);
        }

        public void Stop()
        {
            foreach (var client in connectedClients.ToArray())
            {
                client.Disconnect();
                connectedClients.Remove(client);
            }

            socket.Disconnect(true);
        }

        public void Dispose()
        {
            Stop();
            connectedClients.Clear();
            connectedClients = null;
            socket.Dispose();
            socket = null;
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            var tmpSocket = socket.EndAccept(ar);
            socket.BeginAccept(OnClientAccepted, null);
            tmpSocket.NoDelay = true;

            var client = new ConnectedClient(tmpSocket);
            OnClientConnected?.Invoke(this, client);

            lock (lockObj)
                connectedClients.Add(client);

            client.Start();
        }
    }
}

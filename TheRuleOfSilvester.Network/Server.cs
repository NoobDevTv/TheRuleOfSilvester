using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Network
{
    public class Server : IDisposable
    {
        public event EventHandler<ConnectedClient> OnClientConnected;

        public int ClientAmount => connectedClients.Count;

        private Socket socket;
        private readonly Dictionary<ConnectedClient, GameSession> connectedClients;
        private readonly SemaphoreExtended semaphore;
        private readonly HashSet<ServerSession> sessions;

        public Server()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connectedClients = new Dictionary<ConnectedClient, GameSession>();
            semaphore = new SemaphoreExtended(1, 1);
            sessions = new HashSet<ServerSession>();
        }

        public void Start(IPAddress address, int port)
        {
            sessions.Add(new LobbySession());

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
                client.Key.Disconnect();
                connectedClients.Remove(client.Key);
            }

            socket.Disconnect(true);
        }

        public void Dispose()
        {
            Stop();

            connectedClients.Clear();

            socket.Dispose();
            semaphore.Dispose();

            socket = null;
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            var tmpSocket = socket.EndAccept(ar);
            tmpSocket.NoDelay = true;

            var client = new ConnectedClient(tmpSocket);

            OnClientConnected?.Invoke(this, client);

            using (semaphore.Wait())
            {
                sessions
                    .OfType<LobbySession>()
                    .First()
                    .ConnectedClients
                    .Add(client);
            }

            client.Start();
            client.Send(new byte[] { 1 }, 1);
            socket.BeginAccept(OnClientAccepted, null);
        }
    }
}

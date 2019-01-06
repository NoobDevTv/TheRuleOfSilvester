using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Network
{
    public class Server : IObserver<Package>, IDisposable
    {
        public event EventHandler<ConnectedClient> OnClientConnected;
        public event EventHandler<Package> OnCommandReceived;

        public int ClientAmount => connectedClients.Count;

        private Socket socket;
        private List<ConnectedClient> connectedClients;
        private readonly object lockObj;
        private readonly HashSet<IDisposable> subscriptions;
        private readonly SemaphoreSlim semaphore;

        public Server()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            lockObj = new object();
            semaphore = new SemaphoreSlim(1,1);
            subscriptions = new HashSet<IDisposable>();
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
            tmpSocket.NoDelay = true;

            var client = new ConnectedClient(tmpSocket);
            
            OnClientConnected?.Invoke(this, client);

            lock (lockObj)
                connectedClients.Add(client);

            semaphore.Wait();
            subscriptions.Add(client.Subscribe(this));
            semaphore.Release();

            client.Start();
            socket.BeginAccept(OnClientAccepted, null);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error) 
            => throw error;

        public void OnNext(Package value) 
            => OnCommandReceived?.Invoke(this, value);
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    public class Server : IDisposable
    {
        public event EventHandler<ConnectedClient> OnClientConnected;
        public event EventHandler OnClientDisconnected;

        public int ClientAmount => connectedClients.Count;

        private Socket socket;
        private readonly List<ConnectedClient> connectedClients;
        private readonly SemaphoreExtended semaphore;
        private readonly SessionProvider sessionProvider;
        private readonly PlayerService playerService;
        private readonly Subject<ConnectedClient> clientSubject;
        private readonly SerialDisposable disposables;

        public Server()
        {
            clientSubject = new Subject<ConnectedClient>();
            disposables = new SerialDisposable();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connectedClients = new List<ConnectedClient>();
            semaphore = new SemaphoreExtended(1, 1);
            sessionProvider = new SessionProvider();
            playerService = new PlayerService();
        }

        public void Start(IPAddress address, int port)
        {
            sessionProvider.Add(new LobbyServerSession(sessionProvider, playerService));

            disposables.Disposable = new CompositeDisposable()
            {
                sessionProvider.NewClients(clientSubject)
            };

            socket.Bind(new IPEndPoint(address, port));
            socket.Listen(1024);
            socket.BeginAccept(OnClientAccepted, null);
        }
        public void Start(string host, int port)
        {
            IPAddress address = Dns.GetHostAddresses(host).FirstOrDefault(
                a => a.AddressFamily == socket.AddressFamily);

            Start(address, port);
        }

        public void Stop()
        {
            disposables.Disposable = Disposable.Empty;
            foreach (ConnectedClient client in connectedClients.ToArray())
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

            socket.Dispose();
            semaphore.Dispose();
            disposables.Dispose();

            socket = null;
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            Socket tmpSocket = socket.EndAccept(ar);
            tmpSocket.NoDelay = true;

            var client = new ConnectedClient(tmpSocket);
            connectedClients.Add(client);
            client.OnDisconnected += ClientOnDisconnected;

            OnClientConnected?.Invoke(this, client);

            client.Start();
            clientSubject.OnNext(client);

            //client.Send(new byte[] { 1 }, 1);
            socket.BeginAccept(OnClientAccepted, null);
        }

        private void ClientOnDisconnected(object sender, EventArgs e)
        {
            if(sender is ConnectedClient client)
            {
                connectedClients.Remove(client);
                client.Stop();
                client.Dispose();
            }

            OnClientDisconnected?.Invoke(sender, e);
        }
    }
}

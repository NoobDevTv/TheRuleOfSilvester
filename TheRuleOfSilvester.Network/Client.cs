using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Network
{
    public class Client : BaseClient
    {
        public IObservable<Package> ReceivedPackages => packageSubject;

        private readonly Subject<Package> packageSubject;

        public Client() :
            base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            packageSubject = new Subject<Package>();
        }
        
        public void Connect(string host, int port)
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault(
                a => a.AddressFamily == Socket.AddressFamily);

            Socket.BeginConnect(new IPEndPoint(address, port), OnConnected, null);
        }

        public void Wait()
        {
            //Wait for Connection
            while (!Socket.Connected)
                Thread.Sleep(1);

            var buffer = new byte[1];
            Socket.Receive(buffer);

            if (buffer[0] == 0)
                throw new Exception("Connection Error");
        }

        protected override void CallOnNext(Package package) 
            => packageSubject.OnNext(package);

        private void OnConnected(IAsyncResult ar)
        {
            Socket.EndConnect(ar);
            Start();
        }
    }
}

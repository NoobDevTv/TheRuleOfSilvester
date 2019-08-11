using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Network
{
    public abstract class ServerSession : IObserver<Package>, IDisposable
    {
        public List<ConnectedClient> ConnectedClients { get; }

        public event EventHandler<Package> OnCommandReceived;
        
        public ServerSession()
        {
            ConnectedClients = new List<ConnectedClient>();
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
            => throw error;

        public void OnNext(Package value)
            => OnCommandReceived?.Invoke(this, value);

        public void Dispose() => throw new NotImplementedException();
    }
}

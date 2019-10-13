using System.Collections.Generic;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Network.Sessions
{
    public interface IServerSession
    {
        public int Id { get; set; }
        IReadOnlyCollection<ConnectedClient> ConnectedClients { get; }

        void AddClient(ConnectedClient client);
        void RemoveClient(ConnectedClient client);
    }
}
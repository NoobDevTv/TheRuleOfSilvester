using System.Collections.Generic;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Network.Sessions
{
    public interface IServerSession
    {
        public int Id { get; set; }

        void AddClient(BaseClient client);
        void RemoveClient(BaseClient client);
    }
}
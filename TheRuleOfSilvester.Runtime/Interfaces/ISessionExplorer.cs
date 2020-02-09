using System.Collections.Generic;
using System.Threading;
using TheRuleOfSilvester.Network.Info;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface ISessionExplorer
    {
        GameServerSessionInfo ShowServerSessionDialog(IEnumerable<GameServerSessionInfo> gameServerSessionInfos, CancellationToken token);
    }
}
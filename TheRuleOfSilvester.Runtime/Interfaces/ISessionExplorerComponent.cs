using System.Collections.Generic;
using TheRuleOfSilvester.Network.Info;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface ISessionExplorerComponent
    {
        GameServerSessionInfo ShowServerSessionDialog(IEnumerable<GameServerSessionInfo> gameServerSessionInfos);
    }
}
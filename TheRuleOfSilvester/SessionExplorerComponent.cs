using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Network.Info;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester
{
    public sealed class SessionExplorerComponent : ISessionExplorerComponent
    {
        private readonly SelectionGrid<GameServerSessionInfo> selectionControl;

        public SessionExplorerComponent()
        {
            //selectionControl = new SelectionGrid<GameServerSessionInfo>();
        }

        public GameServerSessionInfo ShowServerSessionDialog(IEnumerable<GameServerSessionInfo> gameServerSessionInfos)
        {
            selectionControl.Clear();
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            var maxNameLength = gameServerSessionInfos.Max(x => x.Name.Length) + 4;
            var str = $"{{0,-{maxNameLength}}} {{1,2}}/{{2,-2}}";
            selectionControl.AddRange(gameServerSessionInfos.Select(x => (x, string.Format(str, x.Name, x.CurrentPlayers, x.MaxPlayers))));
            var ret = selectionControl.ShowModal("Lobby", true);

            return ret;
        }

    }
}

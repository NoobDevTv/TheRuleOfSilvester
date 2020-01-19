using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Network.Info;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.Drawing
{
    public sealed class SessionExplorer : ISessionExplorer
    {
        private readonly SelectionGrid<GameServerSessionInfo> selectionControl;

        public SessionExplorer(ConsoleInput consoleInput)
        {
            selectionControl = new SelectionGrid<GameServerSessionInfo>(consoleInput);
        }

        public GameServerSessionInfo ShowServerSessionDialog(IEnumerable<GameServerSessionInfo> gameServerSessionInfos)
        {
            selectionControl.Clear();
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);

            int maxNameLength = 0;

            if (gameServerSessionInfos.Any())
                maxNameLength = gameServerSessionInfos.Max(x => x.Name.Length) + 4;

            var str = $"{{0,-{maxNameLength}}} {{1,2}}/{{2,-2}}";
            selectionControl.AddRange(gameServerSessionInfos.Select(x => (x, string.Format(str, x.Name, x.CurrentPlayers, x.MaxPlayers))));
            var ret = selectionControl.ShowModal("Lobby", CancellationToken.None, true);

            return ret;
        }

    }
}

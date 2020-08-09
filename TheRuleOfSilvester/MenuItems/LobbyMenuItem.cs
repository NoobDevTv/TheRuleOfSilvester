using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Options;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.MenuItems
{
    internal sealed class LobbyMenuItem : MenuItem
    {
        private readonly Game game;
        private readonly IObservable<ServerStatus> gameStatus;

        public LobbyMenuItem(ConsoleInput consoleInput, Game game, IObservable<ServerStatus> status) 
            : base(consoleInput, "Lobby")
        {
            this.game = game;
            gameStatus = status;
        }

        protected override IObservable<MenuResult> Action(CancellationToken token) 
            => Observable
                    .Return(game)
                    .Do(g => Console.WriteLine("Entered Lobby"))
                    //.Do(g => )
                    .SelectMany(g => gameStatus
                                        .Where(s => s == ServerStatus.Started)
                                        .Select(s => new MenuResult<Game>(game))
                               );
    }
}

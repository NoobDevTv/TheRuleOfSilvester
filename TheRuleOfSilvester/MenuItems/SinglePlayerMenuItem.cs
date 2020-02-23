using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Components;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Runtime;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.MenuItems
{
    internal sealed class SinglePlayerMenuItem : MenuItem
    {
        public SinglePlayerMenuItem(ConsoleInput consoleInput) : base(consoleInput, "New Game")
        {
        }

        protected override IObservable<MenuResult> Action(CancellationToken token) 
            => Observable.Create<MenuResult>((observer, cancelToken) => Task.Run(() =>
                {
                    Console.Clear();
                    var x = GetIntFromUser("Map Width", 10, 400);
                    var y = GetIntFromUser("Map Height", 10, 400);

                    var game = new Game
                    {
                        InputCompoment = new InputComponent(ConsoleInput),
                        DrawComponent = new DrawComponent()
                    };
                    game.RunSinglePlayer(60, x, y);
                    var result = new MenuResult<Game>(game);
                    observer.OnNext(result);
                    game.Wait();
                    observer.OnCompleted();

                }, cancelToken));

        private int GetIntFromUser(string title, int min = 0, int max = int.MaxValue)
        {
            string raw;
            int value;
            do
            {
                Console.Clear();
                Console.Write($"{title} (Leave Empty for default): ");
                raw = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(raw))
                    return min;

            } while (!int.TryParse(raw, out value) || value < min || value > max);

            return value;
        }
    }
}

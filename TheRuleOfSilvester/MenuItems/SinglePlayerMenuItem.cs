using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        protected override Task Action(CancellationToken token)
        {
            Console.Clear();
            var x = GetIntFromUser("Map Width", 10, 400);
            var y = GetIntFromUser("Map Height", 10, 400);

            var game = new Game
            {
                DrawComponent = new DrawComponent(),
                SessionExplorerComponent = new SessionExplorerComponent()
            };

            throw new NotImplementedException();
            //TODO: Run Game
        }

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
                    return 0;

            } while (!int.TryParse(raw, out value) || value < min || value > max);

            return value;
        }
    }
}

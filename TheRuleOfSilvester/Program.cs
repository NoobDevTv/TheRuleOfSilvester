using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Components;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.MenuItems;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester
{
    internal class Program
    {
        //┌┬┐└┴┘│├┼┤
        private static async Task Main(string[] args)
        {
            //are = new AutoResetEvent(false);
            Console.ForegroundColor = ConsoleColor.White;
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;

            var input = new ConsoleInput();
            var exitItem = new ExitMenuItem(input);

            var menu = new SelectionGrid<MenuItem>(input, new List<MenuItem>()
                {
                   new SinglePlayerMenuItem(input),
                   new MultiplayerMenuItem(input),
                   new OptionsMenuItem(input),
                   exitItem
                })
            {
                Name = "MainMenuGrid"
            };

            do
            {
                Console.Clear();
                MenuItem menuItem = menu.ShowModal("The Rule Of Silvester", true);
                IDisposable disposable = null;

                try
                {
                    disposable = await menuItem.Run();
                }
                catch (OperationCanceledException)
                {
                    //No issue
                }

                disposable?.Dispose();
            } while (!exitItem.Token.IsCancellationRequested);
        }
    }
}

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
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;
            
            var input = new ConsoleInput();
            var exitItem = new ExitMenuItem();

            var menu = new SelectionGrid<MenuItem>(new List<MenuItem>()
                {
                   new SinglePlayerMenuItem(),
                   new MultiplayerMenuItem(),
                   new OptionsMenuItem(),
                   exitItem
                });

            do
            {
                Console.Clear();
                MenuItem menuItem = menu.ShowModal("The Rule Of Silvester", true);

                var disposable = await menuItem.Run(input);
                disposable.Dispose();
            } while (!exitItem.Token.IsCancellationRequested);
        }
    }
}

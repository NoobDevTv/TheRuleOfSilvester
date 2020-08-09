using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Components;
using TheRuleOfSilvester.Core.Options;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.MenuItems;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester
{
    internal class Program
    {
        private static AutoResetEvent autoResetEvent;
        private static ManualResetEvent exitResetEvent;
        //┌┬┐└┴┘│├┼┤
        private static void Main(string[] args)
        {
            //are = new AutoResetEvent(false);
            Console.ForegroundColor = ConsoleColor.White;
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;

            autoResetEvent = new AutoResetEvent(false);
            exitResetEvent = new ManualResetEvent(false);

            var optionFile = OptionFile.Load();

            var input = new ConsoleInput();
            var exitItem = new ExitMenuItem(input);

            var menu = new SelectionGrid<MenuItem>(input, new List<MenuItem>()
                {
                   new SinglePlayerMenuItem(input),
                   new MultiplayerMenuItem(input, optionFile),
                   new OptionsMenuItem(input, optionFile),
                   exitItem
                })
            {
                Name = "MainMenuGrid"
            };

            using var menuItemSubject = new Subject<MenuItem>();
            IObservable<MenuItem> menuItemsFromMenu = Observable
                .Return<Func<MenuItem>>(() => menu.ShowModal("The Rule Of Silvester", CancellationToken.None, true))
                .Do(m => Console.Clear())
                .Select(showMenu => showMenu());

            using IDisposable subscription = Observable
                .Merge(menuItemsFromMenu, menuItemSubject)
                .SelectMany(item => item.Run())
                .Do(r => HandleResult(r, menuItemSubject))
                .Repeat()
                .Subscribe();

            exitResetEvent.WaitOne();
        }

        private static void HandleResult(MenuResult result, IObserver<MenuItem> menuItemObserver)
        {
            switch (result)
            {
                case MenuResult<bool> exitResult:
                    exitResetEvent.Set();
                    break;
                case MenuResult<Game> gameResult:
                    gameResult.Content.Wait();
                    break;
                case MenuResult<MenuItem> itemResult:
                    Task.Run(() => menuItemObserver.OnNext(itemResult.Content));
                    break;
                default:
                    break;
            }
        }

        private static void ShowMenu(SelectionGrid<MenuItem> menu)
            => Task.Run(() =>
                {
                    void CallShowMenu()
                    {
                        autoResetEvent.Set();
                        ShowMenu(menu);
                    }

                    Console.Clear();
                    MenuItem menuItem = menu.ShowModal("The Rule Of Silvester", CancellationToken.None, true);

                    IDisposable menuObservable = menuItem
                    .Run()
                    .Subscribe(result =>
                    {
                        switch (result)
                        {
                            case MenuResult<bool> exitResult:
                                exitResetEvent.Set();
                                break;
                            case MenuResult<Game> gameResult:
                                //gameResult.Content.Wait();
                                break;
                            default:
                                break;
                        }
                    }, ex => CallShowMenu(), CallShowMenu);

                    autoResetEvent.WaitOne();
                });
    }
}

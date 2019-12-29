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
        //┌┬┐└┴┘│├┼┤
        private static async Task Main(string[] args)
        {
            //are = new AutoResetEvent(false);
            Console.ForegroundColor = ConsoleColor.White;
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;

            var option = new OptionFile();
            option.Options.Add("Test", new Option(new Demo { A = 12, B = "Test", demo =new Demo { A = 4 } } ));
            option.Options.Add("TestB", new Option(new Demo { A = 12, B = "TestA", } ));
            option.Options.Add("TestC", new Option(new Demo { A = 14 } ));
            option.Options.Add("TestD", new Option( new Demo {  } ));

            var demo = JsonConvert.SerializeObject(option, Formatting.Indented);

            //var optionFile = File.ReadAllText(Path.Combine(".", "options.json"));
            var file = JsonConvert.DeserializeObject<OptionFile>(demo);

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
                MenuItem menuItem = menu.ShowModal("The Rule Of Silvester", exitItem.Token, true);
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

        public class Demo
        {
            public int A { get; set; }
            public string B { get; set; }
            public Demo demo { get; set; }
        }
    }
}

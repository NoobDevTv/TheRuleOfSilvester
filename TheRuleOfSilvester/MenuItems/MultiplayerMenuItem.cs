using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.MenuItems
{
    internal sealed class MultiplayerMenuItem : MenuItem
    {
        private readonly SelectionGrid<MenuItem> selectionGrid;

        public MultiplayerMenuItem(ConsoleInput consoleInput) : base(consoleInput, "Multiplayer")
        {
            selectionGrid = new SelectionGrid<MenuItem>(ConsoleInput, new List<MenuItem>
            {

                new SimpleMenuItem(ConsoleInput,"Join Lobby", null),
                new SimpleMenuItem(ConsoleInput,"Join with IPAddress", null),
                new SimpleMenuItem(ConsoleInput,"Create Global Game", null),
                new SimpleMenuItem(ConsoleInput,"Create Local Game", null)
            })
            {
                Name = "MultiplayerGrid"
            };
        }

        protected override Task Action(CancellationToken token)
        {

            throw new NotImplementedException();
            //do
            //{
            //    Console.Write("Spielername: ");

            //    playerName = Console.ReadLine();
            //    Console.Clear();
            //}
            //while (string.IsNullOrWhiteSpace(playerName));

            //Console.Write("IP Address of Server: ");

            //bool GetAddress(string value, out IPAddress ipAddress)
            //{
            //    //TODO: Port
            //    if (value.ToLower() == "localhorst" || string.IsNullOrWhiteSpace(value) || value.ToLower() == "horst")
            //        value = "localhost";

            //    if (IPAddress.TryParse(value, out ipAddress))
            //        return true;
            //    try
            //    {
            //        ipAddress = Dns.GetHostAddresses(value)
            //            .FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork);
            //    }
            //    catch { }

            //    return ipAddress != null;
            //}

            //IPAddress address;
            //while (!GetAddress(Console.ReadLine(), out address))
            //{
            //    Console.WriteLine("You've entered a wrong ip. Please try again! ☺");
            //    Console.Write("IP Address of Server: ");
            //}
            //Console.Clear();
            //multiplayerComponent.Host = address.ToString();
            //multiplayerComponent.Port = 4400;

            //CreateGame(true);
        }
    }
}

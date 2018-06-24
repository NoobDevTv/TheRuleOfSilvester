using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester
{
    class Program
    {
        private static Game game;
        private static InputComponent inputComponent;
        private static MultiplayerComponent multiplayerComponent;
        private static GameMenu menu;

        public static bool IsRunning { get; private set; }

        //┌┬┐└┴┘│├┼┤
        static void Main(string[] args)
        {
            //are = new AutoResetEvent(false);
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;
            inputComponent = new InputComponent();
            multiplayerComponent = new MultiplayerComponent();
            IsRunning = true;

            menu = new GameMenu(new List<MenuItem>()
            {
               new MenuItem(true, "New Game", SinglePlayer),
               new MenuItem(false, "Multiplayer", MultiPlayer),
               new MenuItem(false, "Exit", () => false)
            });

            while (IsRunning)
            {
                var menuItem = menu.Run();
                IsRunning = menuItem.Action();
            }
        }

        private static bool MultiPlayer()
        {
            Console.Clear();

            Console.Write("IP Address of Server: ");

            bool GetAddress(string value, out IPAddress ipAddress)
            {
                //TODO: Port
                if (value.ToLower() == "localhorst" || string.IsNullOrWhiteSpace(value)|| value.ToLower() == "horst")
                    value = "localhost";

                if (IPAddress.TryParse(value, out ipAddress))
                    return true;
                try
                {
                    ipAddress = Dns.GetHostAddresses(value)
                        .FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork);
                }
                catch { }

                return ipAddress != null;
            }

            IPAddress address;
            while (!GetAddress(Console.ReadLine(), out address))
            {
                Console.WriteLine("You've entered a wrong ip. Please try again! ☺");
                Console.Write("IP Address of Server: ");
            }

            multiplayerComponent.Host = address.ToString();
            multiplayerComponent.Port = 4400;

            CreateGame(true);
            return true;
        }

        private static bool SinglePlayer()
        {
            Console.Clear();
            CreateGame(false);
            return true;
        }

        private static void CreateGame(bool isMultiplayer)
        {
            using (game = new Game())
            {
                game.DrawComponent = new DrawComponent();
                game.InputCompoment = inputComponent;
                game.MultiplayerComponent = multiplayerComponent;
                Console.Write("Choose a character: ");
                var character = Console.ReadLine();
                game.Run(60, 60, isMultiplayer, character);
                inputComponent.Listen();
                game.Stop();
            }
        }
    }
}

using CommandManagementSystem;
using System;
using System.Net;
using System.Threading;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    class Program
    {
        private static DefaultCommandManager<CommandName, CommandArgs, byte[]> manager;
        static Network.Server server;

        static void Main(string[] args)
        {
            using var mResetEvent = new ManualResetEvent(false);
            manager = new DefaultCommandManager<CommandName, CommandArgs, byte[]>(typeof(Program).Namespace + ".Commands");

            using (server = new Network.Server())
            {
                var x = GetIntFromUser("Map Width", 10, 400);
                var y = GetIntFromUser("Map Height", 10, 400);

                GameManager.GenerateMap(x, y);

                server.Start(IPAddress.Any, 4400);
                Console.CancelKeyPress += (s, e) => mResetEvent.Reset();

                server.OnCommandReceived += (s, e) =>
                {
                    var connectedClient = (ConnectedClient)e.Client;
                    NetworkPlayer player = null;

                    if (connectedClient.Registered)
                        GameManager.Players.TryGetValue(connectedClient.PlayerId, out player);

                    e.Data = manager.Dispatch(command: e.CommandName, new CommandArgs(player, connectedClient, e.Data));
                    e.Client.Send(e);
                };

                server.OnClientConnected += ServerOnClientConnected;
                Console.WriteLine("Server has started, waiting for clients");
                string command;
                do
                {
                    command = "";
                    command = Console.ReadLine();
                } while (command.ToLower() != "!start");

                Console.WriteLine("Game started.");
                GameManager.StartGame();
                mResetEvent.WaitOne();
                GameManager.StopGame();
            }
        }

        private static int GetIntFromUser(string title, int min = 0, int max = int.MaxValue)
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

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            Console.WriteLine("New Client has connected. Current Amount: " + (server.ClientAmount + 1));
        }
    }
}

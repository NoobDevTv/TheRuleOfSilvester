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
            var mResetEvent = new ManualResetEvent(false);
            manager = new DefaultCommandManager<CommandName, CommandArgs, byte[]>(typeof(Program).Namespace + ".Commands");

            using (server = new Network.Server())
            {
                server.Start(IPAddress.Any, 4400);
                Console.CancelKeyPress += (s, e) => mResetEvent.Reset();
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

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            e.OnCommandReceived += (s, args) =>
            {
                var connectedClient = (ConnectedClient)s;
                NetworkPlayer player = null;

                if (connectedClient.Registered)
                    GameManager.Players.TryGetValue(connectedClient.PlayerId, out player);

                var answer = manager.Dispatch(command: (CommandName)args.Command, new CommandArgs(player, connectedClient, args.Data));
                
                e.Send(answer, answer.Length);
            };

            Console.WriteLine("New Client has connected. Current Amount: " + server.ClientAmount + 1);
        }
    }
}

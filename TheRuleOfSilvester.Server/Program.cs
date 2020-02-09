using CommandManagementSystem;
using System;
using System.Net;
using System.Threading;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    class Program
    {
        static Server server;

        static void Main(string[] args)
        {
            using var mResetEvent = new ManualResetEvent(false);

            using (server = new Server())
            {
                server.Start(IPAddress.Any, 4400);
                Console.CancelKeyPress += (s, e) => mResetEvent.Reset();
                
                server.OnClientConnected += ServerOnClientConnected;
                server.OnClientDisconnected += ServerOnClientDisconnected;

                Console.WriteLine("Server has started, waiting for clients");
                string command;
                do
                {
                    command = "";
                    command = Console.ReadLine();
                } while (command.ToLower() != "!start");

                Console.WriteLine("Game started.");
                mResetEvent.WaitOne();
            }
        }

        private static void ServerOnClientDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Client has disconnected. Current Amount: " + server.ClientAmount);
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            Console.WriteLine("New Client has connected. Current Amount: " + server.ClientAmount);
        }
    }
}

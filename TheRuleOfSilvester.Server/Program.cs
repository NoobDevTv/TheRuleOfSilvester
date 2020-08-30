using CommandManagementSystem;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Net;
using System.Threading;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    class Program
    {
        static Server server;
        private static Logger logger;

        static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, new ColoredConsoleTarget("server-tros.console"));
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, new FileTarget("server-tros.logfile")
            {
                FileName = $"./logs/server-{DateTime.Now:ddMMyy_hhmmss}.log"
            });
            LogManager.Configuration = config;
            logger = LogManager.GetCurrentClassLogger();

            using var mResetEvent = new ManualResetEvent(false);

            using (server = new Server())
            {
                server.Start(IPAddress.Any, 4400);
                Console.CancelKeyPress += (s, e) => mResetEvent.Reset();
                
                server.OnClientConnected += ServerOnClientConnected;
                server.OnClientDisconnected += ServerOnClientDisconnected;

                logger.Info("Server has started, waiting for clients");
                string command;
                do
                {
                    command = "";
                    command = Console.ReadLine();
                } while (command.ToLower() != "!start");

                logger.Info("Game started.");
                mResetEvent.WaitOne();
            }
        }

        private static void ServerOnClientDisconnected(object sender, EventArgs e)
        {
            logger.Info("Client has disconnected. Current Amount: " + server.ClientAmount);
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            logger.Info("New Client has connected. Current Amount: " + server.ClientAmount);
        }
    }
}

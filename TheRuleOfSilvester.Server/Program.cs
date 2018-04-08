using CommandManagementSystem;
using System;
using System.Net;
using System.Threading;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    class Program
    {
        private static DefaultCommandManager<short, byte[], byte[]> manager;

        static void Main(string[] args)
        {
            var mResetEvent = new ManualResetEvent(false);
            manager = new DefaultCommandManager<short, byte[], byte[]>(typeof(Program).Namespace + ".Commands");

            using (var server = new Network.Server())
            {
                server.Start(IPAddress.Any, 4400);
                Console.CancelKeyPress += (s, e) => mResetEvent.Reset();
                server.OnClientConnected += ServerOnClientConnected;
                mResetEvent.WaitOne();
            }
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
            => e.OnCommandReceived += (s, args) =>
            {
                var answer = manager.Dispatch(command: args.Command, arg: args.Data);
                e.Send(answer, answer.Length);
            };
    }
}

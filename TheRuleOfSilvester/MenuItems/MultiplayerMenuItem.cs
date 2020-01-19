using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Components;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Core.Options;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Network.Info;
using TheRuleOfSilvester.Runtime;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.MenuItems
{
    internal sealed class MultiplayerMenuItem : MenuItem
    {
        private readonly SelectionGrid<MenuItem> selectionGrid;
        private readonly OptionFile options;

        public MultiplayerMenuItem(ConsoleInput consoleInput, OptionFile optionFile) : base(consoleInput, "Multiplayer")
        {
            selectionGrid = new SelectionGrid<MenuItem>(ConsoleInput, new List<MenuItem>
            {
                new SimpleMenuItem(ConsoleInput,"Join Lobby", JoinLobby),
                new SimpleMenuItem(ConsoleInput,"Join with IPAddress", null),
                new SimpleMenuItem(ConsoleInput,"Create Global Game", CreateGlobalGame),
                new SimpleMenuItem(ConsoleInput,"Create Local Game", null)
            })
            {
                Name = "MultiplayerGrid"
            };

            options = optionFile;
        }



        protected override async Task Action(CancellationToken token)
        {
            do
            {
                Console.Clear();
                MenuItem menuItem = selectionGrid.ShowModal(Title, token, true);
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
            } while (!token.IsCancellationRequested);


            //Console.Clear();
            //multiplayerComponent.Host = address.ToString();
            //multiplayerComponent.Port = 4400;

            //CreateGame(true);
        }

        private Task JoinLobby(CancellationToken token)
        {
            var component = GetMultiplayerComponent(token);

            var notification = new[] { (CommandName.GetSessions, new Notification(Array.Empty<byte>(), NotificationType.Sessions)) };
            var selectionGrid = new SessionExplorer(ConsoleInput);

            var sessions = CreateSessionsAndResetEvent(component, 
                SerializeHelper.DeserializeToList<GameServerSessionInfo>, 
                NotificationType.Sessions,
                out var resetEvent)
                    .Select(selectionGrid.ShowServerSessionDialog);

            using (resetEvent)
                return RunAndWait(component, sessions, resetEvent, notification);
        }


        private Task CreateGlobalGame(CancellationToken token)
        {
            var component = GetMultiplayerComponent(token);
            string name = string.Empty;
            int maxPlayers = 0;

            var editableGrid = new EditableGrid<string>(ConsoleInput);
            editableGrid.Add("", "Session name");
            editableGrid.Add("", "Max Players");
            editableGrid.ConvertMethod = (value, display) =>
            {
                var raw = new string(value.ToArray());
                switch (display)
                {
                    case "Session name":
                        name = raw;
                        break;
                    case "Max Players":
                        maxPlayers = int.Parse(raw);
                        break;
                    default:
                        throw new KeyNotFoundException($"{display} didn't exist as Parameter for a session");
                }

                return raw;
            };

            editableGrid.Show("New GameSession", token, vertical: true);
            byte[] data;
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(name);
                writer.Write(maxPlayers);
                data = stream.ToArray();
            }

            var notification = new[] { (CommandName.NewSession, new Notification(data, NotificationType.Sessions)) };
            var sessions = CreateSessionsAndResetEvent(component, 
                SerializeHelper.Deserialize<GameServerSessionInfo>,
                NotificationType.Session,
                out var resetEvent);

            using (resetEvent)
                return RunAndWait(component, sessions, resetEvent, notification);
        }

        private IObservable<T> CreateSessionsAndResetEvent<T>(IMultiplayerComponent component,
            Func<byte[], T> deserializeFunc, NotificationType type, out ManualResetEventSlim resetEvent)
        {
            resetEvent = new ManualResetEventSlim(false);
            return component
                .GetNotifications()
                .Where(n => n.Type == type)
                .Select(n => n.Deserialize(deserializeFunc));
        }

        private Task RunAndWait(IMultiplayerComponent multiplayerComponent, IObservable<GameServerSessionInfo> sessions,
            ManualResetEventSlim resetEvent, (CommandName, Notification)[] notification)
        {
            using var subscription = sessions
                             .Do(session => RunGame(multiplayerComponent, session))
                             .Subscribe(session =>
                             {
                                 resetEvent.Set();
                             });

            multiplayerComponent.Connect();
            using var packages = multiplayerComponent.SendPackages(notification.ToObservable());
            resetEvent.Wait();
            return Task.CompletedTask;
        }

        private void RunGame(IMultiplayerComponent component, GameServerSessionInfo gameServerSessionInfo)
        {
            var game = new Game
            {
                MultiplayerComponent = component,
                InputCompoment = new InputComponent(ConsoleInput),
                DrawComponent = new DrawComponent()
            };
            game.RunMultiplayer(30, gameServerSessionInfo.Id);
            game.Wait();
        }

        private IMultiplayerComponent GetMultiplayerComponent(CancellationToken token)
        {
            var optionValue = options.Options.TryGetValue(OptionKeys.Player, out Option playerNameValue);
            var playerName = playerNameValue.Value as string;

            while (!optionValue && string.IsNullOrWhiteSpace(playerName))
            {
                Console.Clear();
                Console.Write("Playername: ");
                playerName = ConsoleInput.ReadLine(token);
            }

            optionValue = options.Options.TryGetValue(OptionKeys.Host, out Option hostValue);
            var host = hostValue.Value as string;

            IPAddress address;
            while (!GetAddress(host, out address) && !optionValue)
            {
                Console.WriteLine("You've entered a wrong ip. Please try again! ☺");
                Console.Write("IP Address of Server: ");
                host = Console.ReadLine();
            }

            return new MultiplayerComponent
            {
                Host = address.ToString(),
                Port = 4400
            };
        }

        private bool GetAddress(string value, out IPAddress ipAddress)
        {
            //TODO: Port
            if (value.ToLower() == "localhorst" || string.IsNullOrWhiteSpace(value) || value.ToLower() == "horst")
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
    }
}

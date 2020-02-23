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



        protected override IObservable<MenuResult> Action(CancellationToken token)
            => Observable.Create<MenuResult>(observer => Task.Run(() =>
                {
                    Console.Clear();
                    MenuItem menuItem = selectionGrid.ShowModal(Title, token, true);
                    return menuItem.Run().Subscribe(observer);
                }, token));

        private IObservable<MenuResult> JoinLobby(CancellationToken token)
        {
            var component = GetMultiplayerComponent(token);

            var notification = new[] { (CommandName.GetSessions, new Notification(Array.Empty<byte>(), NotificationType.Sessions)) };
            var selectionGrid = new SessionExplorer(ConsoleInput);

            var sessions = CreateSessionsAndResetEvent(component,
                SerializeHelper.DeserializeToList<GameServerSessionInfo>,
                NotificationType.Sessions)
                    .Select(sessions => selectionGrid.ShowServerSessionDialog(sessions, token));

            return RunAndWait(component, sessions, notification);
        }


        private IObservable<MenuResult> CreateGlobalGame(CancellationToken token)
        {
            var component = GetMultiplayerComponent(token);
            string name = string.Empty;
            int maxPlayers = 0;

            var editableGrid = new EditableGrid<string>(ConsoleInput);

            
            editableGrid.Add("", "Session name");
            editableGrid.Add("4", "Max Players");
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

            IObservable<MenuResult> exitObservable = Observable
                 .FromEventPattern(a => editableGrid.OnExit += a, r => editableGrid.OnExit -= r)
                 .Select(e => new MenuResult<object>(null))
                 .Do(m => throw new OperationCanceledException());

            IObservable<MenuResult> saveObservable = Observable
                 .FromEventPattern<IEnumerable<string>>(a => editableGrid.OnSave += a, r => editableGrid.OnSave -= r)
                 .Select(e => e.EventArgs)
                 .SelectMany(o =>
                 {
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
                         NotificationType.Session);

                     return RunAndWait(component, sessions, notification);
                 });

            return Observable.Create<MenuResult>(observer =>
            {
                var sub = Observable.Merge(exitObservable, saveObservable).Subscribe(observer);
                editableGrid.Show("New GameSession", token, vertical: true);
                return sub;
            });
        }

        private IObservable<T> CreateSessionsAndResetEvent<T>(IMultiplayerComponent component,
            Func<byte[], T> deserializeFunc, NotificationType type)
        {
            return component
                .GetNotifications()
                .Where(n => n.Type == type)
                .Select(n => n.Deserialize(deserializeFunc));
        }

        private IObservable<MenuResult> RunAndWait(
            IMultiplayerComponent multiplayerComponent,
            IObservable<GameServerSessionInfo> sessions,
            (CommandName, Notification)[] notification)
        {
            return Observable.Create<MenuResult>(observer =>
            {
                multiplayerComponent.Connect();
                var subscription = sessions
                                 .Select(session =>
                                 {
                                     return new MenuResult<Game>(GetGame(multiplayerComponent, session));
                                 })
                                 .Subscribe(result => observer.OnNext(result), exception =>
                                 {
                                     multiplayerComponent.Disconnect();
                                     observer.OnError(exception);
                                 });

                var sub2 = multiplayerComponent
                            .GetNotifications()
                            .Where(n => n.Type == NotificationType.Success)
                            .Select(n => multiplayerComponent.SendPackages(notification.ToObservable()))
                            .Subscribe();

                return new CompositeDisposable
                {
                    subscription,
                    sub2
                };
            });
        }

        private Game GetGame(IMultiplayerComponent component, GameServerSessionInfo gameServerSessionInfo)
        {
            var game = new Game
            {
                MultiplayerComponent = component,
                InputCompoment = new InputComponent(ConsoleInput),
                DrawComponent = new DrawComponent()
            };
            game.RunMultiplayer(30, gameServerSessionInfo.Id);
            return game;
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

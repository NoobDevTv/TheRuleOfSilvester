using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server
{
    public class NetworkPlayer : IDisposable
    {
        public IPlayer Player { get; set; }
        public RoundMode RoundMode
        {
            get => roundMode;
            internal set
                => SetValue((RoundMode)((int)value % maxRoundMode), ref roundMode, eventInvoke: v => OnRoundModeChanged?.Invoke(this, v));
        }
        public ServerStatus CurrentServerStatus
        {
            get
            {
                var tmp = currentServerStatus;
                if (tmp == ServerStatus.Started)
                    currentServerStatus = ServerStatus.Ok;
                return tmp;
            }
            set => SetValue(value, ref currentServerStatus, eventInvoke: v => OnServerStatusChanged?.Invoke(this, v));
        }
        private ServerStatus currentServerStatus;

        public List<PlayerAction> UpdateSets
        {
            get => updateSets;
            internal set => SetValue(value, ref updateSets, eventInvoke: v => OnPlayerActionsChanged?.Invoke(this, v));
        }
        private List<PlayerAction> updateSets;

        public string PlayerName { get; }

        public event EventHandler<RoundMode> OnRoundModeChanged;
        public event EventHandler<List<PlayerAction>> OnPlayerActionsChanged;
        public event EventHandler<ServerStatus> OnServerStatusChanged;

        private RoundMode roundMode;

        private readonly int maxRoundMode;

        private readonly IDisposable subscription;
        private readonly Subject<(CommandName CommandName, Notification Notification)> notificationSubject;
        private readonly Logger logger;

        public NetworkPlayer(string playerName, BaseClient client)
        {
            logger = LogManager.GetCurrentClassLogger();
            PlayerName = playerName;
            maxRoundMode = Enum.GetValues(typeof(RoundMode)).Cast<int>().Max() + 1;
            UpdateSets = new List<PlayerAction>();
            CurrentServerStatus = ServerStatus.Waiting;

            notificationSubject = new Subject<(CommandName CommandName, Notification Notification)>();

            var status = Observable
                .FromEventPattern<ServerStatus>(a => OnServerStatusChanged += a, r => OnServerStatusChanged -= r)
                .Do(s => logger.Debug("Send Status to Player: " + s.EventArgs.ToString()))
                .Select(eventP => (CommandName.GetStatus, new Notification(SerializeHelper.SerializeEnum(eventP.EventArgs), NotificationType.ServerStatus)));

            var updateSets = Observable
                .FromEventPattern<List<PlayerAction>>(a => OnPlayerActionsChanged += a, r => OnPlayerActionsChanged -= r)
                .Do(s => logger.Debug("Send UpdateSet to Player: " + s.EventArgs.ToString()))
                .Select(eventP => (CommandName.Wait, new Notification(SerializeHelper.SerializeList(eventP.EventArgs), NotificationType.PlayerActions)));

            var updates = Observable.Merge(status, updateSets);
            var packages = notificationSubject
                                .Select(tuple => new Package(tuple.CommandName, tuple.Notification.Serialize()));

            subscription = StableCompositeDisposable.Create(
                client.SendPackages(packages),
                updates.Subscribe(notificationSubject),
                notificationSubject);
        }

        public void StartGame(Map map)
        {
            var players = map.Players.Where(p => p != (PlayerCell)Player);
            notificationSubject.OnNext((CommandName.GetMap, new Notification(SerializeHelper.Serialize(map), NotificationType.Map)));
            notificationSubject.OnNext((CommandName.GetPlayers, new Notification(SerializeHelper.SerializeList(players), NotificationType.Players)));
        }

        public void Dispose()
        {
            subscription.Dispose();
        }

        private void SetValue<T>(T value, ref T field, [CallerMemberName] string caller = "", Action<T> eventInvoke = null)
        {
            if (Equals(value, field))
                return;

            Console.WriteLine("Set value");
            field = value;
            eventInvoke?.Invoke(value);
        }
    }
}

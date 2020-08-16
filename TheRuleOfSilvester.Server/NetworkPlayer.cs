using System;
using System.Collections.Generic;
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
            {
                roundMode = (RoundMode)((int)value % maxRoundMode);
                OnRoundModeChanged?.Invoke(this, roundMode);
            }
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

        public List<PlayerAction> UpdateSets { get; internal set; }
        public string PlayerName { get; }

        public event EventHandler<RoundMode> OnRoundModeChanged;
        public event EventHandler<ServerStatus> OnServerStatusChanged;

        private RoundMode roundMode;

        private readonly int maxRoundMode;

        private readonly IDisposable subscription;
        private readonly Subject<(CommandName CommandName, Notification Notification)> notificationSubject;

        public NetworkPlayer(string playerName, BaseClient client)
        {
            PlayerName = playerName;
            maxRoundMode = Enum.GetValues(typeof(RoundMode)).Cast<int>().Max() + 1;
            UpdateSets = new List<PlayerAction>();
            CurrentServerStatus = ServerStatus.Waiting;

            notificationSubject = new Subject<(CommandName CommandName, Notification Notification)>();

            var status = Observable
                .FromEventPattern<ServerStatus>(a => OnServerStatusChanged += a, r => OnServerStatusChanged -= r)
                .Do(s => Console.WriteLine("Send Status to Player: " + s.EventArgs.ToString()))
                .Select(eventP => (CommandName.GetStatus, new Notification(SerializeHelper.SerializeEnum(eventP.EventArgs), NotificationType.ServerStatus)));

            var packages = notificationSubject
                                .Select(tuple => new Package(tuple.CommandName, tuple.Notification.Serialize()));

            subscription = StableCompositeDisposable.Create(
                client.SendPackages(packages),
                status.Subscribe(notificationSubject),
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

        private void SetValue<T>(T value, ref T field, [CallerMemberName]string caller = "", Action<T> eventInvoke = null)
        {
            if (Equals(value, field))
                return;

            Console.WriteLine("Set value");
            field = value;
            eventInvoke?.Invoke(value);
        }
    }
}

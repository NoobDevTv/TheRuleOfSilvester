using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Runtime;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network.Info;
using TheRuleOfSilvester.Core.Extensions;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using TheRuleOfSilvester.Core.Observation;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Diagnostics;

namespace TheRuleOfSilvester.Components
{
    public class MultiplayerComponent : IMultiplayerComponent, IDisposable
    {
        public Client Client { get; private set; }

        public int Port { get; set; }
        public string Host { get; set; }
        public IObservable<ServerStatus> CurrentServerStatus
            => serverStatusSubject;

        private readonly IDisposable disposables;
        private readonly BehaviorSubject<ServerStatus> serverStatusSubject;
        private readonly IObservable<(CommandName CommandName, Notification)> notifications;

        public MultiplayerComponent()
        {
            Client = new Client();
            serverStatusSubject = new BehaviorSubject<ServerStatus>(ServerStatus.Undefined);

            notifications = Client
                                .ReceivedPackages
                                .Select(p => (p.CommandName, Notification.FromBytes(p.Data)))
                                .Publish()
                                .RefCount();
            disposables = StableCompositeDisposable.Create(
                serverStatusSubject,
                GetNotifications()
                   .Do(s => Debug.WriteLine("Received Notification from Server: "+ s.Notification.Type.ToString()))
                   .Where(n => n.Notification.Type == NotificationType.ServerStatus)
                   .Select(n => n.Notification.Deserialize(SerializeHelper.DeserializeEnum<ServerStatus>))
                   .Do(s => Debug.WriteLine("Received Status from Server " + s.ToString()))
                   .Subscribe(serverStatusSubject)
                );
        }

        public IObservable<(CommandName CommandName, Notification Notification)> GetNotifications()
            => notifications;

        public IDisposable SendPackages(IObservable<(CommandName Command, Notification Notification)> notifications)
            => Client.SendPackages(notifications
                        .Select(c => new Package(c.Command, c.Notification.Serialize())));

        public void Connect() 
            => Client.Connect(Host, Port);

        public void Disconnect() 
            => Client.Disconnect();

        /// <summary>
        /// Waits for an open Connection
        /// </summary>
        /// <exception cref="ArgumentException">If connection refused</exception>
        public void Wait() 
            => Client.Wait();

        public void Dispose()
            => disposables.Dispose();
    }
}

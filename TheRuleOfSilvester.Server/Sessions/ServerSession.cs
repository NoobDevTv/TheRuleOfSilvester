using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Extensions;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Network.Sessions;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server
{
    public abstract class ServerSession : IDisposable, IServerSession
    {
        public int Id { get; set; }

        private readonly ConcurrentDictionary<BaseClient, IDisposable> connectedSubscriptions;
        private readonly Subject<CommandNotification> notificationSubject;

        public ServerSession()
        {
            connectedSubscriptions = new ConcurrentDictionary<BaseClient, IDisposable>();
            notificationSubject = new Subject<CommandNotification>();
        }

        public IDisposable NewClients(IObservable<BaseClient> clients)
            => clients.Subscribe(AddClient);

        public void AddClient(BaseClient client)
        {
            IDisposable registerCommands = RegisterCommands(client.ReceivedPackages
                                       .Select(p => new CommandNotification()
                                       {
                                           CommandName = p.CommandName,
                                           Client = p.Client,
                                           Notification = Notification.FromBytes(p.Data)
                                       }),
                                 out IObservable<CommandNotification> notifciations);

            notifciations = Observable.Merge(notifciations, notificationSubject);

            var disposables = new CompositeDisposable
            {
                registerCommands,
                client.SendPackages(notifciations
                                    .Where(n => n.Client == client || n.Client == null)
                                    .Select(n => new Package(n.CommandName, n.Notification.Serialize())))
            };

            var successful = connectedSubscriptions.TryAdd(client, disposables);
            Send(client, CommandName.JoinedSession, new Notification(successful.GetBytes(), NotificationType.Success));
        }

        public void RemoveClient(BaseClient client)
        {
            if (connectedSubscriptions.TryRemove(client, out IDisposable subscription))
                subscription.Dispose();
        }

        public virtual void Dispose()
        {
            connectedSubscriptions.Values.ForEach(s => s.Dispose());

            connectedSubscriptions.Clear();
        }

        public bool Contains(BaseClient client)
            => connectedSubscriptions.ContainsKey(client);

        protected void Send(BaseClient client, CommandName commandName, Notification notification)
            => Send(new CommandNotification() { Client = client, CommandName = commandName, Notification = notification });
        protected void Send(CommandNotification commandNotification)
            => notificationSubject.OnNext(commandNotification);

        protected abstract IDisposable RegisterCommands(
            IObservable<CommandNotification> commandsout,
            out IObservable<CommandNotification> notifications);

        protected IDisposable RegisterCommand<T>(
            IObservable<CommandNotification> commands,
            out IObservable<CommandNotification> notifications,
            params object[] args) where T : CommandObserver
        {
            var instance = Activator.CreateInstance(typeof(T), args) as CommandObserver;
            notifications = instance.CommandNotifications;
            return new CompositeDisposable()
            {
                 instance,
                 instance.Register(commands)
            };
        }
    }
}

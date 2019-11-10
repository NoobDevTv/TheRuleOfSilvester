using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
        public IReadOnlyCollection<ConnectedClient> ConnectedClients => connectedClients;

        private readonly List<ConnectedClient> connectedClients;
        private readonly ConcurrentDictionary<ConnectedClient, IDisposable> connectedSubscriptions;
        private readonly List<CommandObserver> disposables;

        public ServerSession()
        {
            connectedClients = new List<ConnectedClient>();
            connectedSubscriptions = new ConcurrentDictionary<ConnectedClient, IDisposable>();
            disposables = new List<CommandObserver>();
        }

        public IDisposable NewClients(IObservable<ConnectedClient> clients)
            => clients.Subscribe(AddClient);

        public void AddClient(ConnectedClient client)
        {
            var disposables = new CompositeDisposable
            {
                RegisterCommands(client.ReceivedPackages
                                       .Select(p => new CommandNotification()
                                       {
                                           CommandName = p.CommandName,
                                           Client = p.Client,
                                           Notification = new Notification(p.Data, NotificationType.None)
                                       }),
                                 out var notifciations),

                client.SendPackages(notifciations
                                    .Where(n => n.Client == client)
                                    .Select(n => new Package(n.CommandName, n.Notification.Serialize())))
            };

            connectedSubscriptions.TryAdd(client, disposables);
        }

        public void RemoveClient(ConnectedClient client)
        {
            if (connectedSubscriptions.TryRemove(client, out var subscription))
                subscription.Dispose();
        }
        
        public virtual void Dispose()
        {
            connectedClients.ForEach(c => c.Disconnect());
            connectedSubscriptions.Values.ForEach(s => s.Dispose());

            connectedClients.Clear();
            connectedSubscriptions.Clear();
        }
        
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

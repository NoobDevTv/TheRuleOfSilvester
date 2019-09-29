using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Extensions;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server
{
    public abstract class ServerSession : INotificationObserver<Package>,
        INotificationObservable<CommandNotification>, IDisposable
    {
        public IReadOnlyCollection<ConnectedClient> ConnectedClients => connectedClients;
        public List<INotificationObserver<CommandNotification>> SubscribedCommands { get; }

        private readonly List<ConnectedClient> connectedClients;
        private readonly Dictionary<ConnectedClient, IDisposable> connectedSubscriptions;
        private readonly List<CommandObserver> disposables;

        public ServerSession()
        {
            connectedClients = new List<ConnectedClient>();
            SubscribedCommands = new List<INotificationObserver<CommandNotification>>();
            connectedSubscriptions = new Dictionary<ConnectedClient, IDisposable>();
            disposables = new List<CommandObserver>();
            RegisterCommands();
        }

        public void AddClient(ConnectedClient client)
        {
            connectedClients.Add(client);
            connectedSubscriptions.Add(client, client.Subscribe(this));            
        }

        public void RemoveClient(ConnectedClient client)
        {
            connectedClients.Remove(client);
            IDisposable subscription = connectedSubscriptions[client];
            subscription.Dispose();
            connectedSubscriptions.Remove(client);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
            => throw error;

        public object OnNext(Package value)
        {
            var connectedClient = (ConnectedClient)value.Client;
            NetworkPlayer player = null;

            if (connectedClient.Registered)
                GameManager.Players.TryGetValue(connectedClient.PlayerId, out player);

            value.Data = Dispatch(new CommandNotification()
            {
                CommandName = value.CommandName,
                Arguments = new CommandArgs(player, connectedClient, value.Data)
            });
            value.Client.Send(value);

            return default;
        }

        public virtual void Dispose()
        {
            connectedClients.ForEach(c => c.Disconnect());
            connectedSubscriptions.Values.ForEach(s => s.Dispose());

            connectedClients.Clear();
            connectedSubscriptions.Clear();
        }

        public IDisposable Subscribe(INotificationObserver<CommandNotification> observer)
        {
            SubscribedCommands.Add(observer);
            return new Subscription<CommandNotification>(observer, this);
        }

        public void OnDispose(INotificationObserver<CommandNotification> observer) 
            => SubscribedCommands.Remove(observer);

        protected byte[] Dispatch(CommandNotification notification)
        {
            foreach (INotificationObserver<CommandNotification> command in SubscribedCommands)
            {
                var Value = command.OnNext(notification);

                if (Value is null)
                    continue;

                return SerializeHelper.GetBytes(Value);
            }

            throw new NotSupportedException();
        }

        

        protected abstract void RegisterCommands();

        protected void RegisterCommand<T>() where T : CommandObserver
        {
            var instance = Activator.CreateInstance<T>() as CommandObserver;
            disposables.Add(instance);
            instance.Register(this);
        }
    }
}

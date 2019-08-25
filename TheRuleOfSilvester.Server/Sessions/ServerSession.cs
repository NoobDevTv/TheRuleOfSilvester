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

        public ServerSession()
        {
            connectedClients = new List<ConnectedClient>();
            SubscribedCommands = new List<INotificationObserver<CommandNotification>>();
            connectedSubscriptions = new Dictionary<ConnectedClient, IDisposable>();
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
            var subscription = connectedSubscriptions[client];
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

        public void Dispose()
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
        {
            SubscribedCommands.Remove(observer);
        }

        protected byte[] Dispatch(CommandNotification notification)
        {
            foreach (var command in SubscribedCommands)
            {
                var returnValue = command.OnNext(notification);

                if (returnValue is null)
                    continue;

                switch (returnValue)
                {
                    case byte[] byteArray:
                        return byteArray;
                    case byte b:
                        return new[] { b };
                    case IByteSerializable serializable:
                        return SerializeHelper.Serialize(serializable);
                    case int p:
                        return BitConverter.GetBytes(p);
                    case uint p:
                        return BitConverter.GetBytes(p);
                    case short p:
                        return BitConverter.GetBytes(p);
                    case ushort p:
                        return BitConverter.GetBytes(p);
                    case long p:
                        return BitConverter.GetBytes(p);
                    case ulong p:
                        return BitConverter.GetBytes(p);
                    case float p:
                        return BitConverter.GetBytes(p);
                    case bool p:
                        return BitConverter.GetBytes(p);
                    case double p:
                        return BitConverter.GetBytes(p);
                    case char p:
                        return BitConverter.GetBytes(p);
                    case string s:
                        return Encoding.UTF8.GetBytes(s);
                    default:
                        if (returnValue is IEnumerable enumerable)
                            return SerializeHelper.SerializeList(enumerable);

                        throw new InvalidCastException();
                }
            }

            throw new NotSupportedException();
        }

        protected abstract void RegisterCommands();
    }
}

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

namespace TheRuleOfSilvester
{
    public class MultiplayerComponent : IMultiplayerComponent
    {
        public Client Client { get; private set; }

        public int Port { get; set; }
        public string Host { get; set; }
        public IObservable<ServerStatus> CurrentServerStatus
            => GetNotifications()
               .Where(n => n.Type == NotificationType.ServerStatus)
               .Select(n => n.Deserialize(SerializeHelper.DeserializeEnum<ServerStatus>));
        
        public MultiplayerComponent()
        {
            Client = new Client();
        }

        public IObservable<Notification> GetNotifications() => Client
            .ReceivedPackages
            .Select(p => new Notification(p.Data, NotificationType.None)); //TODO: Get Notification type from message

        public IDisposable SendPackages(IObservable<(CommandName Command, Notification Notification)> notifications)
            => Client.SendPackages(notifications
                        .Select(c => new Package(c.Command, c.Notification.Serialize())));

        public void Connect() 
            => Client.Connect(Host, Port);

        public void Disconnect() 
            => Client.Disconnect();

        public void Wait() 
            => Client.Wait();        
    }
}

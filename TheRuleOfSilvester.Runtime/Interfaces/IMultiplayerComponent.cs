using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Network.Info;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface IMultiplayerComponent
    {
        Client Client { get; }
        int Port { get; }
        string Host { get; }
        IObservable<ServerStatus> CurrentServerStatus { get; }

        void Connect();

        void Disconnect();

        void Wait();
                
        IObservable<Notification> GetNotifications();
        IDisposable SendPackages(IObservable<(CommandName Command, Notification Notification)> notifications);
    }
}

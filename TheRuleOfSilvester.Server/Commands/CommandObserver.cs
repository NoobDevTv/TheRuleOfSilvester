using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    public abstract class CommandObserver : IDisposable
    {
        public IObservable<CommandNotification> CommandNotifications => commandSubject;

        private readonly ConcurrentDictionary<CommandName, Action<BaseClient, Notification>> commands;
        private readonly Subject<CommandNotification> commandSubject;

        public CommandObserver()
        {
            commandSubject = new Subject<CommandNotification>();
            commands = new ConcurrentDictionary<CommandName, Action<BaseClient, Notification>>();
        }

        public virtual void Dispose()
        {
        }
        
        public void Send(BaseClient client, Notification notification)
        {
            commandSubject.OnNext(new CommandNotification()
            {
                Client = client,
                Notification = notification
            });
        }
        
        public abstract IDisposable Register(IObservable<CommandNotification> observable);
                
        protected bool TryAddCommand(CommandName commandName, Action<BaseClient, Notification> action)
            => commands.TryAdd(commandName, action);
                
        protected bool TryDispatch(CommandName commandName, BaseClient client, Notification notification)
        {
            if (commands.TryGetValue(commandName, out var action))
                action(client, notification);
            else
                return false;

            return true;
        }
        protected bool TryDispatch(CommandNotification commandNotification)
            => TryDispatch(commandNotification.CommandName, commandNotification.Client, commandNotification.Notification);

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core.Observation;

namespace TheRuleOfSilvester.Server
{
    public abstract class CommandObserver : INotificationObserver<CommandNotification>, IDisposable
    {
        protected ServerSession ServerSession;

        private IDisposable subscription;

        public virtual void OnCompleted()
        {
        }

        public virtual void OnError(Exception error)
        {
        }

        public virtual object OnNext(CommandNotification value)
        {
            return default;
        }

        public virtual void Dispose()
        {
            subscription?.Dispose();
        }

        public void Register(INotificationObservable<CommandNotification> observable)
        {
            subscription = observable.Subscribe(this);

            if (observable is ServerSession serverSession)
                ServerSession = serverSession;
        }
    }
}

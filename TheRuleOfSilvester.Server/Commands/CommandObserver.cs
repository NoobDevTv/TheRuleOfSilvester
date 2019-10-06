using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core.Observation;

namespace TheRuleOfSilvester.Server
{
    public abstract class CommandObserver : INotificationObserver<CommandNotification>, IDisposable
    {        
        protected GameManager GameManager;
        protected INotificationObservable<CommandNotification> Observable;

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

        public void Register(INotificationObservable<CommandNotification> observable, GameManager gameManager)
        {
            subscription = observable.Subscribe(this);
            Observable = observable;
            GameManager = gameManager;
        }
    }
}

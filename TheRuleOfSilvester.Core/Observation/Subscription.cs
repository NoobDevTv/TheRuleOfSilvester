using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Observation
{
    public sealed class Subscription<T> : IDisposable
    {
        public INotificationObserver<T> Observer { get; }
        public INotificationObservable<T> Observable { get; }

        public Subscription(INotificationObserver<T> observer, INotificationObservable<T> observable)
        {
            Observer = observer;
            Observable = observable;
        }

        public void Dispose()
        {
            Observable.OnDispose(Observer);
        }
    }
}

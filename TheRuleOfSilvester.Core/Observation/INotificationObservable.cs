using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Observation
{
    public interface INotificationObservable<T>
    {
        IDisposable Subscribe(INotificationObserver<T> observer);
        void OnDispose(INotificationObserver<T> observer);
    }
}

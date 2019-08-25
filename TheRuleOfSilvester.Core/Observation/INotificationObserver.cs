using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Observation
{
    public interface INotificationObserver<T> 
    {
        void OnCompleted();
        void OnError(Exception error);
        object OnNext(T value);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.Core.Observation
{
    interface IAsyncNotificationObservable<T> where T : class
    {
        Task<IDisposable> Subscribe(IObserver<T> observer, CancellationToken token);

        Task OnDispose(INotificationObserver<T> observer, CancellationToken token);
    }
}

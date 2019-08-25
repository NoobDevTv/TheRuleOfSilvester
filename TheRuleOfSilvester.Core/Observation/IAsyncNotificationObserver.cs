using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.Core.Observation
{
    interface IAsyncNotificationObserver<T>
    {
        Task OnCompleted(CancellationToken token);
        Task OnError(Exception error, CancellationToken token);
        Task<object> OnNext(T value, CancellationToken token);
    }
}

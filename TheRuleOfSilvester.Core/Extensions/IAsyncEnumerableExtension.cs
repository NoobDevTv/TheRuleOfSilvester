using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.Core.Extensions
{
    public static class IAsyncEnumerableExtension
    {
        public static async IAsyncEnumerable<T> OnEachAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, Task<T>> func)
        {
            await foreach(var item in enumerable)
                yield return await func(item);    
        }
    }
}

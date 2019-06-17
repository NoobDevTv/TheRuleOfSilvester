using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.Core.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }

            return enumerable;
        }
    }
}

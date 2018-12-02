using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Server
{
    internal class ActionCache : IEnumerable<PlayerAction>
    {
        private readonly ConcurrentBag<PlayerAction> internalBag;

        public ActionCache()
        {
            internalBag = new ConcurrentBag<PlayerAction>();
        }

        public void Add(PlayerAction action)
            => internalBag.Add(action);
        public bool TryGet(out PlayerAction action)
            => internalBag.TryPeek(out action);

        public bool TryNext(out PlayerAction action)
            => internalBag.TryTake(out action);

        internal void AddRange(IEnumerable<PlayerAction> playerActions)
        {
            foreach (var action in playerActions)
                internalBag.Add(action);
        }

        public IEnumerator<PlayerAction> GetEnumerator()
            => new CacheEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public class CacheEnumerator : IEnumerator<PlayerAction>
        {
            private readonly ActionCache actionCache;

            public CacheEnumerator(ActionCache actionCache)
            {
                this.actionCache = actionCache;
            }

            public PlayerAction Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                var result = actionCache.TryNext(out PlayerAction playerAction);
                Current = playerAction;
                return result;
            }

            public void Reset()
            {
                Current = null;
            }

            public void Dispose()
            {
                Reset();
            }
        }
    }
}

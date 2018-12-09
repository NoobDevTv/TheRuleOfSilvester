using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Server
{
    internal class ActionCache : IEnumerable<List<PlayerAction>>
    {
        private readonly ConcurrentDictionary<int, Queue<PlayerAction>> internalDictionary;

        public ActionCache()
        {
            internalDictionary = new ConcurrentDictionary<int, Queue<PlayerAction>>();
        }

        public bool TryAdd(int playerId, PlayerAction action)
        {
            if (!internalDictionary.TryGetValue(playerId, out Queue<PlayerAction> queue))
            {
                if (internalDictionary.ContainsKey(playerId))
                {
                    return false;
                }
                else
                {
                    queue = new Queue<PlayerAction>();
                    if (!internalDictionary.TryAdd(playerId, queue))
                        return false;
                }
            }

            queue.Enqueue(action);

            return true;
        }
        public bool TryGet(int playerId, out PlayerAction action)
        {
            action = null;

            if (!internalDictionary.TryGetValue(playerId, out Queue<PlayerAction> queue))
                return false;

            if (queue.TryPeek(out action))
                return false;

            return true;
        }

        public bool TryNext(out List<PlayerAction> actions)
        {
            actions = new List<PlayerAction>();

            foreach (var playerStack in internalDictionary)
            {
                if (playerStack.Value.TryDequeue(out PlayerAction playerAction))
                    actions.Add(playerAction);
            }

            return actions.Count > 0;
        }

        internal void AddRange(IEnumerable<PlayerAction> playerActions)
        {
            foreach (var action in playerActions)
                TryAdd(action.Player.Id, action);
        }

        public IEnumerator<List<PlayerAction>> GetEnumerator()
            => new CacheEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public class CacheEnumerator : IEnumerator<List<PlayerAction>>
        {
            private readonly ActionCache actionCache;

            public CacheEnumerator(ActionCache actionCache)
            {
                this.actionCache = actionCache;
            }

            public List<PlayerAction> Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                var result = actionCache.TryNext(out List<PlayerAction> playerActions);
                Current = playerActions;
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

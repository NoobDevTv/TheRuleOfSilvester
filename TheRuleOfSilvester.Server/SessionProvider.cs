using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    public sealed class SessionProvider : ICollection<ServerSession>
    {
        private readonly Dictionary<int, ServerSession> sessions;
        private readonly SemaphoreExtended semaphore;

        private Task enqueueTask;

        public SessionProvider()
        {
            sessions = new Dictionary<int, ServerSession>();
            semaphore = new SemaphoreExtended(1, 1);
        }

        public int Count
        {
            get
            {
                using (semaphore.Wait())
                {
                    return sessions.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public void Add(ServerSession item)
        {
            using (semaphore.Wait())
            {
                item.Id = sessions.Keys.LastOrDefault() + 1;
                sessions.Add(item.Id, item);
            }
        }

        public void Clear()
        {
            using (semaphore.Wait())
            {
                sessions.Clear();
            }
        }
                
        public bool Contains(int id)
        {
            using (semaphore.Wait())
            {
                return sessions.ContainsKey(id);
            }
        }
        public bool Contains(ServerSession item)
            => Contains(item.Id);

        public void CopyTo(ServerSession[] array, int arrayIndex)
        {
            using (semaphore.Wait())
            {
                sessions.Values.CopyTo(array, arrayIndex);
            }
        }       

        public bool Remove(ServerSession item)
        {
            using (semaphore.Wait())
            {
                return sessions.Remove(item.Id);
            }
        }

        public void EnqueueSessionChange(int sessionId, ConnectedClient client, ServerSession currentSession)
        {
            if(enqueueTask == null)
            {
                enqueueTask = Task.Run(async () =>
                {
                    await InternalEnqueue(sessionId, client, currentSession);
                });
            }
            else
            {
                enqueueTask.ContinueWith(async (o) =>
                {
                    await InternalEnqueue(sessionId, client, currentSession);
                });
            }
        }

        public IEnumerator<ServerSession> GetEnumerator()
        {
            using (semaphore.Wait())
            {
                return new Enumerator(sessions.Values.ToArray());
            }
        }

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();


        private Task InternalEnqueue(int sessionId, ConnectedClient client, ServerSession currentSession)
        {
            using (semaphore.Wait())
            {
                currentSession?.RemoveClient(client);
                sessions[sessionId].AddClient(client);

                return Task.CompletedTask;
            }
        }

        private struct Enumerator: IEnumerator<ServerSession>
        {
            private readonly ServerSession[] sessions;
            private int index;

            public Enumerator(ServerSession[] sessions)
            {
                this.sessions = sessions;
                index = 0;
            }

            public ServerSession Current => sessions[index];

            object IEnumerator.Current => Current;
            
            public bool MoveNext()
            {
                var newIndex = index + 1;
                var result = newIndex < sessions.Length;

                if (result)
                    index = newIndex;

                return result;
            }

            public void Reset()
            {
                index = 0;
            }
            
            public void Dispose()
            {

            }
        }
    }
}

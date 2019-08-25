using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.Core
{
    public sealed class SemaphoreExtended : IDisposable
    {
        private readonly SemaphoreSlim semaphore;

        public SemaphoreExtended(int initialCount, int maxCount)
        {
            semaphore = new SemaphoreSlim(initialCount, maxCount);
        }


        public SemaphoreLock Wait()
        {
            semaphore.Wait();
            return new SemaphoreLock(this);
        }

        public async Task<SemaphoreLock> WaitAsync(CancellationToken token)
        {
            try
            {
                await semaphore.WaitAsync(token);
            }
            catch
            {
                return new SemaphoreLock(null);
            }

            return new SemaphoreLock(this);
        }

        private void Release()
        {
            semaphore.Release();
        }

        public void Dispose()
        {
            semaphore.Dispose();
        }

        public struct SemaphoreLock : IDisposable
        {
            private readonly SemaphoreExtended semaphore;

            public SemaphoreLock(SemaphoreExtended extendedSemaphore)
            {
                semaphore = extendedSemaphore;
            }

            public void Dispose()
            {
                semaphore?.Release();
            }
        }
    }
}

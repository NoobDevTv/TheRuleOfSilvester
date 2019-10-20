using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Network
{
    public class Awaiter : IDisposable
    {
        public int PackageId { get; set; }
        public byte[] Data { get; set; }
        public bool Successfull { get; set; }

        private readonly ManualResetEventSlim resetEventSlim;

        public Awaiter(int id)
        {
            PackageId = id;
            resetEventSlim = new ManualResetEventSlim(false);
        }

        public void WaitOn()
            => resetEventSlim.Wait();

        public void SetResult(byte[] data, bool successfull)
        {
            Successfull = successfull;
            Data = data;
            resetEventSlim.Set();
        }

        public void Dispose()
        {
            resetEventSlim.Dispose();
            Data = null;
            Successfull = false;
        }
    }
}

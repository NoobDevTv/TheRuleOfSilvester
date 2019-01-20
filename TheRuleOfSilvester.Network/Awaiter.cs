using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Network
{
    public class Awaiter
    {
        public int PackageId { get; set; }
        public byte[] Data { get; set; }
        private readonly ManualResetEventSlim resetEventSlim;

        public Awaiter(int id)
        {
            PackageId = id;
            resetEventSlim = new ManualResetEventSlim(false);
        }

        public void WaitOn()
            => resetEventSlim.Wait();

        public void SetResult(byte[] data)
        {
            Data = data;
            resetEventSlim.Set();
        }
    }
}

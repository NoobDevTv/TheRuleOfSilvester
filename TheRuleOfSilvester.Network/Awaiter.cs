using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Network
{
    public class Awaiter
    {
        public byte[] Data { get; set; }
        private readonly ManualResetEventSlim resetEventSlim;

        public Awaiter()
        {
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

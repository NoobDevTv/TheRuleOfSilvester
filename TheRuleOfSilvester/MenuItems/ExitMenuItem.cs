using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.MenuItems
{
    internal sealed class ExitMenuItem : MenuItem, IDisposable
    {
        public CancellationToken Token { get; }

        private readonly CancellationTokenSource source;

        public ExitMenuItem(ConsoleInput consoleInput) : base(consoleInput, "Exit")
        {
            source = new CancellationTokenSource();
            Token = source.Token;
        }

        protected override Task Action(CancellationToken token)
        {
            source.Cancel();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}

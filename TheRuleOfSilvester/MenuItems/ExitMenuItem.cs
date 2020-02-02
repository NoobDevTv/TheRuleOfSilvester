using System;
using System.Collections.Generic;
using System.Reactive.Linq;
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

        protected override IObservable<MenuResult> Action(CancellationToken token) 
            => Observable.Create<MenuResult>(observer => Task.Run(() =>
                {
                    observer.OnNext(new MenuResult<bool>(true));
                }));

        public void Dispose()
        {
            source.Dispose();
        }
    }
}

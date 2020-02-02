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
    internal sealed class SimpleMenuItem : MenuItem
    {
        private readonly Func<CancellationToken, IObservable<MenuResult>> action;

        public SimpleMenuItem(ConsoleInput consoleInput, string title, Func<CancellationToken, IObservable<MenuResult>> action) : base(consoleInput, title)
        {
            this.action = action;
        }

        protected override IObservable<MenuResult> Action(CancellationToken token)
            => Observable.Create<MenuResult>(observer => () => action(token).Subscribe(observer));
    }
}

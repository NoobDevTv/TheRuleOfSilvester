
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.MenuItems
{
    internal abstract class MenuItem
    {
        public string Title { get; }
        protected ConsoleInput ConsoleInput { get; private set; }

        protected MenuItem(ConsoleInput consoleInput, string title)
        {
            Title = title;
            ConsoleInput = consoleInput;
        }

        public IObservable<MenuResult> Run()
        {
            Console.Clear();
            var compositeDisposable = new CompositeDisposable();

            var cancelationSource = new CancellationTokenSource();

            var disposable = ConsoleInput
                 .ReceivedKeys
                 .Where(i => i.Key == ConsoleKey.Escape)
                 .Subscribe(i => cancelationSource.Cancel());

            compositeDisposable.Add(cancelationSource);
            compositeDisposable.Add(disposable);

            return Action(cancelationSource.Token);
        }

        public override string ToString()
            => Title;

        protected abstract IObservable<MenuResult> Action(CancellationToken token);
    }
}

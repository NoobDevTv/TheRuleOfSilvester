using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.UI.Views;

namespace TheRuleOfSilvester.UI
{
    public class RouterOutlet : IDisposable
    {
        public View CurrentView { get; private set; }

        private readonly SerialDisposable serialDisposable;

        public RouterOutlet()
        {
            serialDisposable = new SerialDisposable();
        }

        public void Show(View view)
        {
            CurrentView = view;
            serialDisposable.Disposable = view.Show().Subscribe();
        }

        public void Dispose()
        {
            serialDisposable.Dispose();
        }
    }
}

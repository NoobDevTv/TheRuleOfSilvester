using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.IoC;

namespace TheRuleOfSilvester.UI
{
    public class App : IDisposable
    {
        private readonly ITypeContainer typeContainer;
        private readonly Router router;
        private readonly ManualResetEvent manualReset;

        public App()
        {
            typeContainer = TypeContainer.Get<ITypeContainer>();

            router = new Router();
            manualReset = new ManualResetEvent(false);

            typeContainer.Register(singelton: router);
        }

        public void Run()
        {
            router.ShowOutlet();
            manualReset.WaitOne();
        }

        public void Dispose()
        {
            router.Dispose();
            manualReset.Dispose();

            if (typeContainer is IDisposable disposable)
                disposable.Dispose();
        }
    }
}

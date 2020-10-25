﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.IoC;
using TheRuleOfSilvester.UI.Drawing;
using TheRuleOfSilvester.UI.Inputs;

namespace TheRuleOfSilvester.UI
{
    public class App : IDisposable
    {
        private readonly ITypeContainer typeContainer;
        private readonly Router router;
        private readonly ManualResetEvent manualReset;

        public App()
        {
            manualReset = new ManualResetEvent(false);
            router = new Router();

            var input = new Input(ConsoleDriver.ReadKey, ConsoleDriver.ReadLine);
            var graphic = new Graphic(
                ConsoleDriver.Write, 
                ConsoleDriver.WriteLine, 
                ConsoleDriver.ForegroundColor, 
                ConsoleDriver.BackgroundColor, 
                ConsoleDriver.SetCursorPosition);

            typeContainer = TypeContainer.Get<ITypeContainer>();

            typeContainer.Register(singelton: input);
            typeContainer.Register(singelton: graphic);
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

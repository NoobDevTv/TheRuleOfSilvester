using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.MenuItems
{
    internal sealed class OptionsMenuItem : MenuItem
    {
        public OptionsMenuItem() : base("Options")
        {
        }

        protected override async Task Action(CancellationToken token)
        {
            Console.Clear();
            Console.WriteLine("Options");

            var test = await ConsoleInput.ReadLine();
            Console.WriteLine("You: " + test);
            token.WaitHandle.WaitOne();
        }
    }
}

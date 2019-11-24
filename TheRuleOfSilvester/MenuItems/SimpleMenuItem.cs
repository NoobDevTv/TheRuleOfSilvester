using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.MenuItems
{
    internal sealed class SimpleMenuItem : MenuItem
    {
        private readonly Action action;

        public SimpleMenuItem(string title, Action action) : base(title)
        {
            this.action = action;
        }

        protected override Task Action(CancellationToken token)
            => Task.Run(action);
    }
}

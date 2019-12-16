using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.MenuItems
{
    internal sealed class OptionsMenuItem : MenuItem
    {
        private readonly EditableGrid<string> editableGrid;

        public OptionsMenuItem(ConsoleInput consoleInput) : base(consoleInput, "Options")
        {
            editableGrid = new EditableGrid<string>(consoleInput) { 
                Name = "OptionsGrid"
            };
            editableGrid.Add("localhost", "Servername");
            editableGrid.Add("TessilRx", "Spielername");
        }

        protected override Task Action(CancellationToken token)
        {
            editableGrid.Show("Options", vertical: true, clearConsole: true);
            token.WaitHandle.WaitOne();
            return Task.CompletedTask;
        }
    }
}

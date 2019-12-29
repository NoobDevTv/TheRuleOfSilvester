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
    internal sealed class OptionsMenuItem : MenuItem
    {
        private readonly EditableGrid<string> editableGrid;
        private readonly ManualResetEventSlim resetEvent;

        public OptionsMenuItem(ConsoleInput consoleInput) : base(consoleInput, "Options")
        {
            resetEvent = new ManualResetEventSlim(false);
            editableGrid = new EditableGrid<string>(consoleInput) { 
                Name = "OptionsGrid"
            };
            editableGrid.Add("localhost", "Servername");
            editableGrid.Add("TessilRx", "Spielername");
        }

        protected override Task Action(CancellationToken token)
        {            
            Observable
                .FromEventPattern(a => editableGrid.OnExit += a, r => editableGrid.OnExit -= r)
                .Subscribe(o =>  resetEvent.Set());
            Observable
                .FromEventPattern<IEnumerable<string>>(a => editableGrid.OnSave += a, r => editableGrid.OnSave -= r)
                .Subscribe(o => resetEvent.Set());

            editableGrid.Show("Options", token, vertical: true, clearConsole: true);

            resetEvent.Wait(token);
            resetEvent.Reset();
            return Task.CompletedTask;
        }
    }
}

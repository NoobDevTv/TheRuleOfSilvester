using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core.Options;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.MenuItems
{
    internal sealed class OptionsMenuItem : MenuItem
    {
        private readonly EditableGrid<OptionValue> editableGrid;
        private readonly ManualResetEventSlim resetEvent;

        public OptionsMenuItem(ConsoleInput consoleInput, OptionFile optionFile) : base(consoleInput, "Options")
        {
            resetEvent = new ManualResetEventSlim(false);
            editableGrid = new EditableGrid<OptionValue>(consoleInput)
            {
                Name = "OptionsGrid",
                ConvertMethod = ConvertInput
            };

            editableGrid.AddRange(optionFile
                                    .Options
                                    .Select(o => (new OptionValue(o.Key, o.Value), o.Key)));
        }

        private OptionValue ConvertInput(IEnumerable<char> input, string key)
        {
            var value = ValueConverter.Parse(new string(input.ToArray()));
            return new OptionValue(key, new Option(value));
        }

        protected override Task Action(CancellationToken token)
        {
            Observable
                .FromEventPattern(a => editableGrid.OnExit += a, r => editableGrid.OnExit -= r)
                .Subscribe(o => resetEvent.Set());
            Observable
                .FromEventPattern<IEnumerable<OptionValue>>(a => editableGrid.OnSave += a, r => editableGrid.OnSave -= r)
                .Subscribe(o => resetEvent.Set());

            editableGrid.Show("Options", token, vertical: true, clearConsole: true);

            resetEvent.Wait(token);
            resetEvent.Reset();
            return Task.CompletedTask;
        }

        private class OptionValue : IEnumerable<char>
        {
            public string OptionKey { get; private set; }
            public Option Option { get; private set; }

            public OptionValue(string optionKey, Option option)
            {
                OptionKey = optionKey;
                Option = option;
            }

            public IEnumerator<char> GetEnumerator()
                => Option.Value.ToString().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

            public override string ToString()
                => Option.Value.ToString();

        }
    }
}

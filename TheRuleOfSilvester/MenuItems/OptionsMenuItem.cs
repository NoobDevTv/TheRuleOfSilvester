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
        private readonly OptionFile optionFile;

        public OptionsMenuItem(ConsoleInput consoleInput, OptionFile optionFile) : base(consoleInput, "Options")
        {
            editableGrid = new EditableGrid<OptionValue>(consoleInput)
            {
                Name = "OptionsGrid",
                ConvertMethod = ConvertInput
            };

            this.optionFile = optionFile;
            editableGrid.AddRange(optionFile
                                    .Options
                                    .Select(o => (new OptionValue(o.Key, o.Value), o.Key)));
        }

        private OptionValue ConvertInput(IEnumerable<char> input, string key)
        {
            var value = ValueConverter.Parse(new string(input.ToArray()));
            return new OptionValue(key, new Option(value));
        }

        protected override IObservable<MenuResult> Action(CancellationToken token)
        {
            IObservable<MenuResult> exitObservable = Observable
                 .FromEventPattern(a => editableGrid.OnExit += a, r => editableGrid.OnExit -= r)
                 .Select(e => new MenuResult<object>(null));
            IObservable<MenuResult> saveObservable = Observable
                 .FromEventPattern<IEnumerable<OptionValue>>(a => editableGrid.OnSave += a, r => editableGrid.OnSave -= r)
                 .Select(e => e.EventArgs)
                 .Select(o =>
                 {
                     o.ForEach(value =>
                     {
                         if (optionFile.Options.TryGetValue(value.OptionKey, out Option oldValue))
                         {
                             optionFile.Options.TryUpdate(value.OptionKey, value.Option, oldValue);
                         }
                     });
                     optionFile.Save();
                     return new MenuResult<OptionFile>(optionFile);
                 });

            return Observable.Create<MenuResult>(observer =>
             {
                 var sub = Observable.Merge(exitObservable, saveObservable).Subscribe(m => observer.OnCompleted());
                 editableGrid.Show(Title, token, vertical: true, clearConsole: true);
                 return sub;
             });
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

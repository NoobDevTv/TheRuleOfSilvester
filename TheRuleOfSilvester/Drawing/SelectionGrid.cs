using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheRuleOfSilvester.Drawing
{
    public sealed class SelectionGrid<T> : Grid<T>
    {
        public override IEnumerable<ConsoleKey> ConsoleKeys => new[]
        {
            ConsoleKey.Enter,
            ConsoleKey.Select,
            ConsoleKey.Spacebar,
            ConsoleKey.UpArrow,
            ConsoleKey.DownArrow,
            ConsoleKey.LeftArrow,
            ConsoleKey.RightArrow
        };

        public SelectionGrid(ConsoleInput consoleInput) : base(consoleInput)
        { }
        public SelectionGrid(ConsoleInput consoleInput, IEnumerable<T> values) 
            : base(consoleInput, values)
        { }
        public SelectionGrid(ConsoleInput consoleInput, IEnumerable<(T Value, string DisplayValue)> values) 
            : base(consoleInput, values)
        { }

        public override void Draw(string instructions, bool vertical = false, bool clearConsole = true)
        {
            base.Draw(instructions, vertical, clearConsole);
            Select(clearConsole);
        }
        public T ShowModal(string instructions, bool vertical = false, bool clearConsole = true)
        {
            using var sub = inputObservable.Subscribe();
            Showing = true;
            base.Draw(instructions, vertical, clearConsole);
            Select(clearConsole);
            Showing = false;
            return Current;
        }
    }
}

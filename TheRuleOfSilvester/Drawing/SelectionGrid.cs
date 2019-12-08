using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheRuleOfSilvester.Drawing
{
    public sealed class SelectionGrid<T> : Grid<T>
    {
        public SelectionGrid(ConsoleInput consoleInput) : base(consoleInput)
        { }
        public SelectionGrid(ConsoleInput consoleInput, IEnumerable<T> values) 
            : base(consoleInput, values)
        { }
        public SelectionGrid(ConsoleInput consoleInput, IEnumerable<(T Value, string DisplayValue)> values) 
            : base(consoleInput, values)
        { }

        public override void ShowModal(string instructions, bool vertical = false, bool clearConsole = true)
        {
            base.ShowModal(instructions, vertical, clearConsole);
            Draw(clearConsole);
        }
        public T ShowModalAndReturn(string instructions, bool vertical = false, bool clearConsole = true)
        {
            base.ShowModal(instructions, vertical, clearConsole);
            Draw(clearConsole);
            return Current;
        }
    }
}

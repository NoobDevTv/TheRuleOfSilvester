using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Drawing
{
    public sealed class EditableGrid<T> : Grid<T>
    {
        public EditableGrid(ConsoleInput consoleInput) : base(consoleInput)
        {
        }

        public EditableGrid(ConsoleInput consoleInput, IEnumerable<(T Value, string DisplayValue)> values)
            : base(consoleInput, values)
        { }

        public override void ShowModal(string instructions, bool vertical = false, bool clearConsole = true)
        {
            base.ShowModal(instructions, vertical, clearConsole);


            foreach (var locationItem in ConsoleLocationItems)
            {
                if (string.IsNullOrEmpty(locationItem.Item.Display))
                {
                    SetConsoleCursor(locationItem.Position);
                    Console.Write(locationItem.Item.Value);
                }
                else
                {
                    SetConsoleCursor(locationItem.Position);
                    Console.Write($"{locationItem.Item.Display}: {locationItem.Item.Value}");
                }
            }
        }
    }
}

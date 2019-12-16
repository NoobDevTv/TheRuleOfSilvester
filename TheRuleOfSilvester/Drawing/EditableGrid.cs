using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.Drawing
{
    public sealed class EditableGrid<T> : Grid<T> where T : IEnumerable<char>
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

        public EditableGrid(ConsoleInput consoleInput) : base(consoleInput)
        {
        }

        public EditableGrid(ConsoleInput consoleInput, IEnumerable<(T Value, string DisplayValue)> values)
            : base(consoleInput, values)
        { }

        public override void Show(string instructions, bool vertical = false, bool clearConsole = true)
        {
            Showing = true;
            var pos = (Console.CursorLeft, Console.CursorTop);
            do
            {
                Console.SetCursorPosition(pos.CursorLeft, pos.CursorTop);
                using (var sub = inputObservable.Subscribe())
                    Draw(instructions, vertical, clearConsole);
                HandleSelected(CurrentItem);
            } while (ContinueLoop(CurrentItem));
            Reset();
            Showing = false;
        }

        public override void Draw(string instructions, bool vertical = false, bool clearConsole = true)
        {
            base.Draw(instructions, vertical, clearConsole);

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

            Console.SetCursorPosition(0, Console.CursorTop + 2);

            ConsoleLocationItems.Add(((Console.CursorLeft, Console.CursorTop), new Item(default, "Exit")));
            Console.Write("Exit");

            Console.SetCursorPosition(7, Console.CursorTop);
            ConsoleLocationItems.Add(((Console.CursorLeft, Console.CursorTop), new Item(default, "Save")));
            Console.Write("Save");

            Select(false);



        }

        private void HandleSelected(Item selected)
        {
            var cli = ConsoleLocationItems.FirstOrDefault(x => x.Item == selected);
            var leftBox = cli.Position.Left + cli.Item.Display.Length + 2;
            var rightBox = cli.Item.Value.ToString().Length + leftBox;

            Console.SetCursorPosition(rightBox, cli.Position.Top);
            var value = Task.Run(async () => await Input.ReadLine(cli.Item.Value.ToString(), CancellationToken.None, true));
            value.Wait();
            var input = value.Result;
            var item = Items.FirstOrDefault(x => x == cli.Item);
            var index = Items.IndexOf(item);
            Items.Remove(item);
            Items.Insert(index, new Item((T)input.AsEnumerable(), cli.Item.Display));
        }

        private bool ContinueLoop(Item item) => item.Display != "Exit" && item.Display != "Save";

        protected override void DrawSelected((int Left, int Top) pos, Item display, bool selected)
        {
            SetConsoleCursor(pos);
            Console.ForegroundColor = selected ? ConsoleColor.Green : ConsoleColor.White;
            if (!(display.Value is null) && !display.Value.Equals(default))
                Console.Write((selected ? ">" : "") + display.Display + ": " + display.Value + (selected ? "<" : "  "));
            else
                base.DrawSelected(pos, display, selected);
        }

    }
}

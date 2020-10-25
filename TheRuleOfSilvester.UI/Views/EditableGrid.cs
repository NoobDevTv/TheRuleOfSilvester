using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.UI.Views
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

        public Func<IEnumerable<char>, string, T> ConvertMethod { get; set; }

        public event EventHandler<IEnumerable<T>> OnSave;
        public event EventHandler OnExit;

        public EditableGrid(ConsoleInput consoleInput) : base(consoleInput)
        {
            ConvertMethod = (c, s) => (T)c;
        }

        public EditableGrid(ConsoleInput consoleInput, IEnumerable<(T Value, string DisplayValue)> values)
            : base(consoleInput, values)
        {
            ConvertMethod = (c, s) => (T)c;
        }

        public override void Show(string instructions, CancellationToken token, bool vertical = false, bool clearConsole = true)
        {
            token.Register(Reset);
            Showing = true;
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);

            do
            {
                token.ThrowIfCancellationRequested();
                Console.SetCursorPosition(CursorLeft, CursorTop);
                
                using (var sub = inputObservable.Subscribe())
                    Draw(instructions, token, vertical, clearConsole);

                switch (CurrentItem)
                {
                    case Item item:
                        HandleSelected(item);
                        break;
                    case OptionsItem optionsItem:
                        HandleOption(optionsItem);
                        break;
                }

                
            } while (ContinueLoop(CurrentItem));

            Reset();
        }

        public override void Draw(string instructions, CancellationToken token, bool vertical = false, bool clearConsole = true)
        {
            base.Draw(instructions, token, vertical, clearConsole);

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

            ConsoleLocationItems.Add(((Console.CursorLeft, Console.CursorTop), new OptionsItem("Exit", Option.Exit)));
            Console.Write("Exit");

            Console.SetCursorPosition(7, Console.CursorTop);
            ConsoleLocationItems.Add(((Console.CursorLeft, Console.CursorTop), new OptionsItem("Save", Option.Save)));
            Console.Write("Save");

            Select(token, false);

        }

        private void HandleSelected(IItem selected)
        {
            var cli = ConsoleLocationItems.FirstOrDefault(x => x.Item.Display == selected.Display);
            var leftBox = cli.Position.Left + cli.Item.Display.Length + 2;
            var rightBox = cli.Item.Value.ToString().Length + leftBox;

            Console.SetCursorPosition(rightBox, cli.Position.Top);
            //var value = Task.Run(async () => await Input.ReadLineAsync(cli.Item.Value.ToString(), CancellationToken.None, true));
            //value.Wait();
            var input = Input.ReadLine(cli.Item.Value.ToString(), CancellationToken.None, true);
            var item = Items.FirstOrDefault(x => x == cli.Item);
            var index = Items.IndexOf(item);
            Items.Remove(item);
            var value = ConvertMethod(input.AsEnumerable(), cli.Item.Display);
            Items.Insert(index, new Item(value, cli.Item.Display));
        }



        private void HandleOption(OptionsItem item)
        {
            switch (item.Option)
            {
                case Option.Exit:
                    OnExit?.Invoke(this, EventArgs.Empty);
                    break;
                case Option.Save:
                    OnSave?.Invoke(this, Items.Select(i => i.Value));
                    break;
            }
        }

        private bool ContinueLoop(IItem item) 
            => !(item is OptionsItem);

        protected override void DrawSelected((int Left, int Top) pos, IItem display, bool selected)
        {
            SetConsoleCursor(pos);
            Console.ForegroundColor = selected ? ConsoleColor.Green : ConsoleColor.White;
            if (!(display.Value is null) && !display.Value.Equals(default))
                Console.Write((selected ? ">" : "") + display.Display + ": " + display.Value + (selected ? "<" : "  "));
            else
                base.DrawSelected(pos, display, selected);
        }

        private readonly struct OptionsItem : IItem, IEquatable<OptionsItem>
        {
            public readonly T Value => default;
            public readonly string Display { get; }
            public readonly Option Option { get;  }

            public OptionsItem(string display, Option option)
            {
                Display = display;
                Option = option;
            }

            public override bool Equals(object obj) 
                => obj is OptionsItem item 
                && Equals(item);

            public bool Equals(OptionsItem other)
                => Display == other.Display 
                && Option == other.Option;

            public override int GetHashCode() 
                => HashCode.Combine(Display, Option);

            public static bool operator ==(OptionsItem left, OptionsItem right) 
                => left.Equals(right);

            public static bool operator !=(OptionsItem left, OptionsItem right) 
                => !(left == right);
        }

        private enum Option
        {
            Exit,
            Save
        }

    }
}

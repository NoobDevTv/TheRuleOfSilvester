using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.Drawing
{
    public abstract class Grid<T>
    {
        public T Current => ConsoleLocationItems[CurrentIndex].Item.Value;
        protected Item CurrentItem => ConsoleLocationItems[CurrentIndex].Item;
        protected (int Left, int Top) CurrentPosition => ConsoleLocationItems[CurrentIndex].Position;
        protected string CurrentDisplayText => ConsoleLocationItems[CurrentIndex].Item.Display;

        protected int CurrentIndex { get; set; }
        protected int UpDownValue { get; set; }

        protected List<((int Left, int Top) Position, Item Item)> ConsoleLocationItems { get; }
        protected List<Item> Items { get; }

        private ConsoleKeyInfo currentKey;

        private readonly ConsoleInput input;
        private readonly IDisposable subscription;
        private readonly ManualResetEventSlim onKeyPressed;

        protected Grid(ConsoleInput consoleInput)
        {
            onKeyPressed = new ManualResetEventSlim();
            Items = new List<Item>();
            ConsoleLocationItems = new List<((int Left, int Top) Position, Item Item)>();
            CurrentIndex = 0;
            UpDownValue = 0;

            input = consoleInput;
            subscription = input.ReceivedKeys.Subscribe(k =>
            {
                currentKey = k;
                onKeyPressed.Set();
            });
        }

        protected Grid(ConsoleInput consoleInput, IEnumerable<T> values) : this(consoleInput)
            => AddRange(values);
        protected Grid(ConsoleInput consoleInput, IEnumerable<(T Value, string DisplayValue)> values) : this(consoleInput)
            => AddRange(values);

        public virtual void Add(T value)
            => Add(value, value.ToString());
        public virtual void Add(T value, string displayValue)
            => Items.Add(new Item(value, displayValue));

        public virtual void AddRange(IEnumerable<(T Value, string DisplayValue)> values)
            => Items.AddRange(values.Select(v => new Item(v.Value, v.DisplayValue)));

        public virtual void AddRange(IEnumerable<T> values)
            => Items.AddRange(values.Select(v => new Item(v, v.ToString())));

        public void Clear()
            => Items.Clear();

        public virtual void ShowModal(string instructions, bool vertical = false, bool clearConsole = true)
        {
            if (!string.IsNullOrWhiteSpace(instructions))
            {
                Console.WriteLine(instructions);
                Console.WriteLine();
            }

            BuildConsoleLocationOfItems(vertical);
        }

        protected virtual void Select(ConsoleKeyInfo pressedKey)
        {
            switch (pressedKey.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (CurrentIndex - 1 < 0)
                        CurrentIndex = ConsoleLocationItems.Count - 1;
                    else
                        CurrentIndex--;
                    SetConsoleCursor(CurrentPosition);
                    break;
                case ConsoleKey.UpArrow:
                    if (CurrentIndex - UpDownValue < 0)
                        CurrentIndex = 0;
                    else
                        CurrentIndex -= UpDownValue;
                    SetConsoleCursor(CurrentPosition);
                    break;
                case ConsoleKey.RightArrow:
                    if (CurrentIndex + 1 >= ConsoleLocationItems.Count)
                        CurrentIndex = ConsoleLocationItems.Count - 1;
                    else
                        CurrentIndex++;
                    SetConsoleCursor(CurrentPosition);
                    break;
                case ConsoleKey.DownArrow:
                    if (CurrentIndex + UpDownValue >= ConsoleLocationItems.Count)
                        CurrentIndex = ConsoleLocationItems.Count - 1;
                    else
                        CurrentIndex += UpDownValue;
                    SetConsoleCursor(CurrentPosition);
                    break;
            }
        }

        /// <summary>
        /// Set the Console Cursor Position
        /// </summary>
        /// <param name="positon">Tuple of the position</param>
        protected virtual void SetConsoleCursor((int Left, int Top) positon)
        {
            Console.SetCursorPosition(positon.Left, positon.Top);
        }

        protected virtual void BuildConsoleLocationOfItems(bool vertical)
        {
            ConsoleLocationItems.Clear();
            var oldPos = (Console.CursorLeft, Console.CursorTop);
            var maxLengthName = Items.Max(x => x.Display.Length) + 2;
            foreach (var item in Items)
            {
                ConsoleLocationItems.Add(
                    ((Console.CursorLeft, Console.CursorTop), item));

                Console.Write(item.Display.PadRight(maxLengthName, ' '));

                if (maxLengthName + 2 + Console.CursorLeft > Console.WindowWidth || vertical)
                    Console.WriteLine();
            }
            SetConsoleCursor(oldPos);
            UpDownValue = ConsoleLocationItems.Count(x => x.Position.Top == ConsoleLocationItems.First().Position.Top);
        }

        protected virtual void DrawSelected((int Left, int Top) pos, string display, bool selected)
        {
            SetConsoleCursor(pos);
            Console.ForegroundColor = selected ? ConsoleColor.Green : ConsoleColor.White;
            Console.Write((selected ? ">" : "") + display + (selected ? "<" : "  "));
        }

        protected virtual void GetSelected(out bool selected, out ConsoleKeyInfo pressedKey)
        {
            onKeyPressed.Wait();
            pressedKey = currentKey;
            selected = pressedKey.Key == ConsoleKey.Enter || pressedKey.Key == ConsoleKey.Select || pressedKey.Key == ConsoleKey.Spacebar;
            onKeyPressed.Reset();
        }

        /// <summary>
        /// Creates a Selection Grid for based on the string values Array
        /// </summary>
        /// <param name="instructions">Printed before the selection grid</param>
        /// <param name="items">List of values to be choosen from</param>
        /// <param name="clearConsole">Console Clear command after selection</param>
        /// <returns>Choosen value</returns>
        protected virtual void Draw(bool clearConsole = true)
        {
            (int Left, int Top) startPos = ConsoleLocationItems.First().Position;

            bool selected;
            SetConsoleCursor(startPos);
            do
            {
                DrawSelected(CurrentPosition, CurrentDisplayText, true);
                GetSelected(out selected, out var pressedKey);

                DrawSelected(CurrentPosition, CurrentDisplayText, false);
                Select(pressedKey);

            } while (!selected);

            if (clearConsole)
            {
                Console.Clear();
            }
            else
            {
                Console.CursorTop = ConsoleLocationItems.Max(x => x.Position.Top);
                Console.WriteLine();
            }
        }

        protected readonly struct Item
        {
            public readonly T Value { get; }
            public readonly string Display { get; }

            public Item(T value, string display)
            {
                Value = value;
                Display = display;
            }
        }
    }
}

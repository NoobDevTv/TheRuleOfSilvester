using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Diagnostics;
using TheRuleOfSilvester.Core;
using System.Runtime.CompilerServices;
using TheRuleOfSilvester.UI.Drawing;
using TheRuleOfSilvester.UI.Inputs;
using System.Reactive.Subjects;
using System.Drawing;

namespace TheRuleOfSilvester.UI.Views
{
    public abstract class Grid<T> : View
    {
        public string Name { get; set; }

        public abstract IEnumerable<ConsoleKey> ConsoleKeys { get; }

        public T Current => CurrentItem.Value;
        public bool Showing { get; protected set; }

        public string Instructions { get; set; }

        protected IItem CurrentItem => ConsoleLocationItems[CurrentIndex].Item;
        protected (int Left, int Top) CurrentPosition => ConsoleLocationItems[CurrentIndex].Position;
        protected string CurrentDisplayText => ConsoleLocationItems[CurrentIndex].Item.Display;

        protected int CurrentIndex { get; set; }
        protected int UpDownValue { get; set; }

        protected List<((int Left, int Top) Position, IItem Item)> ConsoleLocationItems { get; }
        protected List<IItem> Items { get; }
        protected Input Input { get; }
        protected Graphic Graphic { get; }


        protected readonly IObservable<ConsoleKeyInfo> inputObservable;
        private ConsoleKeyInfo currentKey;
        private readonly ManualResetEventSlim onKeyPressed;

        private readonly SemaphoreExtended semaphore;

        private readonly Subject<string> writeLine;
        private readonly Subject<string> write;
        private readonly Subject<Point> cursor;
        private readonly Subject<System.Drawing.Color> foreground;

        protected Grid(Graphic graphic, Input input) : base(Observable.Empty<ViewState>())
        {
            semaphore = new SemaphoreExtended(1, 1);
            onKeyPressed = new ManualResetEventSlim(false);
            Items = new List<IItem>();
            ConsoleLocationItems = new List<((int Left, int Top) Position, IItem Item)>();
            CurrentIndex = 0;
            UpDownValue = 0;

            writeLine = new Subject<string>();
            write = new Subject<string>();
            cursor = new Subject<Point>();
            foreground = new Subject<System.Drawing.Color>();

            Input = input;
            Graphic = graphic;

            inputObservable = Input
                .ReceivedKeys
                .Where(k => ConsoleKeys.Contains(k.Key))
                .Do(k =>
                {
                    using (semaphore.Wait())
                        currentKey = k;

                    onKeyPressed.Set();
                });


        }

        public virtual void Add(T value)
            => Add(value, value.ToString());
        public virtual void Add(T value, string displayValue)
            => Items.Add(new Item(value, displayValue));

        public virtual void AddRange(IEnumerable<(T Value, string DisplayValue)> values)
            => Items.AddRange(values.Select(v => new Item(v.Value, v.DisplayValue) as IItem));

        public virtual void AddRange(IEnumerable<T> values)
            => Items.AddRange(values.Select(v => new Item(v, v.ToString()) as IItem));

        public void Clear()
            => Items.Clear();

        public virtual void Show(string instructions, CancellationToken token, bool vertical = false, bool clearConsole = true)
        {
            token.Register(Reset);
            using var sub = inputObservable.Subscribe();
            Showing = true;
            Draw(instructions, token, vertical, clearConsole);
            Reset();
        }

        protected override void OnShow()
        {

        }

        protected override void OnStateChange(ViewState oldState, ViewState currentState)
        {
        }

        public virtual void Draw(string instructions, CancellationToken token, bool vertical = false, bool clearConsole = true)
        {
            if (!Showing)
                return;

            if (!string.IsNullOrWhiteSpace(instructions))
            {
                writeLine.OnNext(instructions);
                writeLine.OnNext("");
            }

            BuildConsoleLocationOfItems(vertical);
        }

        protected virtual void IndexSelect(ConsoleKeyInfo pressedKey)
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
        protected virtual void SetConsoleCursor(Point positon)
        {
            cursor.OnNext(positon);
        }

        protected virtual IObservable<Point> BuildConsoleLocationOfItems(IObservable<Point> cursorPosition, bool vertical)
        {
            return cursorPosition.Select(cursorPos =>
            {
                ConsoleLocationItems.Clear();
                var oldPos = cursorPos;

                int maxLengthName = 0;
                if (Items.Count > 0)
                    maxLengthName = Items.Max(x => x.Display.Length) + 2;

                foreach (var item in Items)
                {
                    ConsoleLocationItems.Add(
                        ((Console.CursorLeft, Console.CursorTop), item));

                    write.OnNext(item.Display.PadRight(maxLengthName, ' '));

                    if (maxLengthName + 2 + Console.CursorLeft > Console.WindowWidth || vertical)
                        writeLine.OnNext("");
                }

                UpDownValue = ConsoleLocationItems.Count(x => x.Position.Top == ConsoleLocationItems.First().Position.Top);
                return oldPos;
            });
        }

        protected virtual void DrawSelected((int Left, int Top) pos, IItem display, bool selected)
        {
            SetConsoleCursor(pos);
            foreground.OnNext(selected ? System.Drawing.Color.Green : System.Drawing.Color.White);
            write.OnNext((selected ? ">" : "") + display.Display + (selected ? "<" : "  "));
        }

        protected virtual void GetSelected(CancellationToken token, out bool selected, out ConsoleKeyInfo pressedKey)
        {
            onKeyPressed.Wait(token);
            onKeyPressed.Reset();

            using (semaphore.Wait())
            {
                pressedKey = currentKey;
                selected = pressedKey.Key == ConsoleKey.Enter || pressedKey.Key == ConsoleKey.Select || pressedKey.Key == ConsoleKey.Spacebar;
                currentKey = default;
            }
        }

        /// <summary>
        /// Creates a Selection Grid for based on the string values Array
        /// </summary>
        /// <param name="instructions">Printed before the selection grid</param>
        /// <param name="items">List of values to be choosen from</param>
        /// <param name="clearConsole">Console Clear command after selection</param>
        /// <returns>Choosen value</returns>
        protected virtual IItem Select(CancellationToken token, bool clearConsole = true)
        {
            (int Left, int Top) startPos = ConsoleLocationItems.First().Position;

            bool selected;
            SetConsoleCursor(startPos);
            do
            {
                token.ThrowIfCancellationRequested();
                DrawSelected(CurrentPosition, CurrentItem, true);
                GetSelected(token, out selected, out var pressedKey);

                DrawSelected(CurrentPosition, CurrentItem, false);
                IndexSelect(pressedKey);

            } while (!selected);

            if (clearConsole)
            {
                Console.Clear();
            }
            else
            {
                cursor.OnNext(new Point(ConsoleLocationItems.Max(x => x.Position.Top), 0));
                writeLine.OnNext("");
            }
            return CurrentItem;
        }

        protected virtual void Reset()
        {
            CurrentIndex = 0;
            Showing = false;

            foreground.OnNext(System.Drawing.Color.White);
        }

        protected readonly struct Item : IItem, IEquatable<Item>
        {
            public readonly T Value { get; }
            public readonly string Display { get; }

            public Item(T value, string display)
            {
                Value = value;
                Display = display;
            }

            public override bool Equals(object obj)
                => obj is Item item
                   && Equals(item);
            public bool Equals(Item other)
                => EqualityComparer<T>.Default.Equals(Value, other.Value)
                   && Display == other.Display;

            public override int GetHashCode()
                => HashCode.Combine(Value, Display);

            public static bool operator ==(Item left, Item right)
                => left.Equals(right);
            public static bool operator !=(Item left, Item right)
                => !(left == right);
        }

        public interface IItem
        {
            T Value { get; }
            string Display { get; }
        }
    }
}

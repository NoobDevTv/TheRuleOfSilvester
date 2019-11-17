using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheRuleOfSilvester.Drawing
{
    public sealed class SelectionGrid<T>
    {
        private readonly List<Item> items;

        public SelectionGrid()
            => items = new List<Item>();
        public SelectionGrid(IEnumerable<T> values) : this()
            => AddRange(values);
        public SelectionGrid(IEnumerable<(T Value, string DisplayValue)> values) : this()
            => AddRange(values);

        public void Add(T value)
            => Add(value, value.ToString());
        public void Add(T value, string displayValue)
            => items.Add(new Item(value, displayValue));

        public void AddRange(IEnumerable<(T Value, string DisplayValue)> values)
            => items.AddRange(values.Select(v => new Item(v.Value, v.DisplayValue)));
        public void AddRange(IEnumerable<T> values)
            => items.AddRange(values.Select(v => new Item(v, v.ToString())));

        public void Clear()
            => items.Clear();

        public T ShowModal(string instructions, bool vertical = false, bool clearConsole = true)
        {
            //if (typeof(T) is Type)
            //    return (T)(object)CreateSelectionForTypes(instructions, vertical, clearConsole);

            return CreateForArray(instructions, vertical, clearConsole);
        }

        /// <summary>
        /// Creates a Selection Grid for based on the string values Array
        /// </summary>
        /// <param name="instructions">Printed before the selection grid</param>
        /// <param name="items">List of values to be choosen from</param>
        /// <param name="clearConsole">Console Clear command after selection</param>
        /// <returns>Choosen value</returns>
        private T CreateForArray(string instructions, bool vertical = false, bool clearConsole = true)
        {
            if (!string.IsNullOrWhiteSpace(instructions))
            {
                Console.WriteLine(instructions);
                Console.WriteLine();
            }
            (int Left, int Top) startPos = (Console.CursorLeft, Console.CursorTop);
            var consoleLocationItems = new List<((int Left, int Top) pos, Item selection)>();
            var maxLengthName = items.Max(x => x.Display.Length) + 2;
            foreach (var item in items)
            {

                consoleLocationItems.Add(
                    ((Console.CursorLeft, Console.CursorTop), item));

                Console.Write(item.Display.PadRight(maxLengthName, ' ') );

                if (maxLengthName + 2 + Console.CursorLeft > Console.WindowWidth || vertical)
                    Console.WriteLine();
            }

            var updownValue = consoleLocationItems.Count(x => x.pos.Top == consoleLocationItems.First().pos.Top);

            bool selected;
            int current = 0;
            SetConsoleCursor(startPos);
            do
            {
                SetConsoleCursor(consoleLocationItems[current].pos);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(">" + consoleLocationItems[current].selection.Display + "<");

                var pressedKey = Console.ReadKey(true);
                selected = pressedKey.Key == ConsoleKey.Enter || pressedKey.Key == ConsoleKey.Select || pressedKey.Key == ConsoleKey.Spacebar;

                SetConsoleCursor(consoleLocationItems[current].pos);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(consoleLocationItems[current].selection.Display + "  ");
                switch (pressedKey.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (current - 1 < 0)
                            current = consoleLocationItems.Count - 1;
                        else
                            current--;
                        SetConsoleCursor(consoleLocationItems[current].pos);
                        break;
                    case ConsoleKey.UpArrow:
                        if (current - updownValue < 0)
                            current = 0;
                        else
                            current -= updownValue;
                        SetConsoleCursor(consoleLocationItems[current].pos);
                        break;
                    case ConsoleKey.RightArrow:
                        if (current + 1 >= consoleLocationItems.Count)
                            current = consoleLocationItems.Count - 1;
                        else
                            current++;
                        SetConsoleCursor(consoleLocationItems[current].pos);
                        break;
                    case ConsoleKey.DownArrow:
                        if (current + updownValue >= consoleLocationItems.Count)
                            current = consoleLocationItems.Count - 1;
                        else
                            current += updownValue;
                        SetConsoleCursor(consoleLocationItems[current].pos);
                        break;
                    default:
                        break;
                }

            } while (!selected);
            if (clearConsole)
                Console.Clear();
            Console.CursorTop = consoleLocationItems.Max(x => x.pos.Top);
            Console.WriteLine();
            return consoleLocationItems[current].selection.Value;
        }

        /// <summary>
        /// Creates a Selection Grid for based on a list of types
        /// </summary>
        /// <param name="instructions">Printed before the selection grid</param>
        /// <param name="items">List of types to be choosen from</param>
        /// <param name="clearConsole">Console Clear command after selection</param>
        /// <returns>Choosen type</returns>
        //private Type CreateSelectionForTypes(string instructions, bool vertical = false, bool clearConsole = true)
        //{
        //    var res = CreateForArray(instructions, items.Select(x => x.Name), clearConsole);
        //    return items.FirstOrDefault(x => x.Name == res);
        //}

        /// <summary>
        /// Set the Console Cursor Position
        /// </summary>
        /// <param name="positon">Tuple of the position</param>
        private void SetConsoleCursor((int Left, int Top) positon)
        {
            Console.CursorLeft = positon.Left;
            Console.CursorTop = positon.Top;
        }

        /// <summary>
        /// Creates a Selection Grid for all enum values with the names
        /// </summary>
        /// <typeparam name="T">Enum for the values</typeparam>
        /// <param name="instructions">Printed before the selection grid</param>
        /// <param name="clearConsole">Console Clear command after selection</param>
        /// <returns>Choosen enum value</returns>
        //private T CreateSelectionGridForEnum(string instructions, bool clearConsole = true) where T : Enum
        //{
        //    var res = CreateForArray(instructions, Enum.GetNames(typeof(T)).Select(x => x), clearConsole);
        //    return (T)Enum.Parse(typeof(T), res);
        //}

        private readonly struct Item
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

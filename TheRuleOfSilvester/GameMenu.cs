using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheRuleOfSilvester
{
    class GameMenu
    {
        public List<MenuItem> MenueItems { get; private set; }
        public bool IsRunning { get; private set; }

        public GameMenu(List<MenuItem> items)
        {
            MenueItems = items;
        }

        public MenuItem Run()
        {
            IsRunning = true;

            while (IsRunning)
            {
                DrawMenu();
                var menuItem = MenuAction(Console.ReadKey());
                if (menuItem != null)
                    return menuItem;
            }

            return null;
        }

        public void DrawMenu()
        {
            Console.Clear();

            for (int i = 0; i < MenueItems.Count; i++)
            {
                if (MenueItems[i].Selected)
                    Console.WriteLine($">{i + 1}. {MenueItems[i].Title}");
                else
                    Console.WriteLine($"{i + 1}. {MenueItems[i].Title}");
            }
        }

        private MenuItem MenuAction(ConsoleKeyInfo consoleKeyInfo)
        {
            int index;
            MenuItem item;

            switch (consoleKeyInfo.Key)
            {
                case ConsoleKey.Tab:
                case ConsoleKey.DownArrow:
                    index = MenueItems.FindIndex(i => i.Selected);
                    item = MenueItems[index];
                    item.Selected = false;
                    item = MenueItems[(index + 1) % MenueItems.Count];
                    item.Selected = true;
                    return null;
                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    return MenueItems.First(m => m.Selected);
                case ConsoleKey.UpArrow:
                    index = MenueItems.FindIndex(i => i.Selected);
                    item = MenueItems[index];
                    item.Selected = false;
                    item = MenueItems[(index + MenueItems.Count - 1) % MenueItems.Count];
                    item.Selected = true;
                    return null;
                default:
                    return null;
            }

        }
    }
}

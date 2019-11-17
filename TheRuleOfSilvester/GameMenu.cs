using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Drawing;

namespace TheRuleOfSilvester
{
    class GameMenu
    {
        public List<MenuItem> MenueItems { get; private set; }
        public bool IsRunning { get; private set; }
        private SelectionGrid<MenuItem> selectionGrid;

        public GameMenu(List<MenuItem> items)
        {
            MenueItems = items;
            selectionGrid = new SelectionGrid<MenuItem>(items);
        }

        public MenuItem Run()
        {
            IsRunning = true;

            while (IsRunning)
            {
                var menuItem = selectionGrid.ShowModal("", true);
                if (menuItem != null)
                    return menuItem;
            }

            return null;
        }
    }
}

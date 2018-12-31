
using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester
{
    internal class MenuItem
    {
        public bool Selected { get; set; }
        public string Title { get; set; }
        public Action Action { get; set; }

        public MenuItem(string title) => Title = title;
        public MenuItem(bool selected, string title) : this(title) => Selected = selected;
        public MenuItem(bool selected, string title, Action action) : this(selected, title) => Action = action;
    }
}

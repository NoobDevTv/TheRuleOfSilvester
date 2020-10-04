using System;
using System.Collections.Generic;
using TheRuleOfSilvester.UI.Controls;

namespace TheRuleOfSilvester.UI.Views
{
    public abstract class View : GraphicsElement<ViewState>
    {
        private readonly List<Control> controls;

        public View(IObservable<ViewState> viewStates)
            : base(viewStates)
        {
            controls = new List<Control>();
        }       

        public void Add(Control control)
        {
            controls.Add(control);
        }

        public void Remove(Control control)
        {
            controls.Remove(control);
        }
    }
}

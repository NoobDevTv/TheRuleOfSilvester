using System;
using System.Collections.Generic;

namespace TheRuleOfSilvester.UI.Views
{
    public abstract class View : GraphicsElement<ViewState>
    {
        private readonly List<View> views;

        public View(IObservable<ViewState> viewStates)
            : base(viewStates)
        {
            views = new List<View>();
        }       

        public void Add(View view)
        {
            views.Add(view);
        }

        public void Remove(View view)
        {
            views.Remove(view);
        }
    }
}

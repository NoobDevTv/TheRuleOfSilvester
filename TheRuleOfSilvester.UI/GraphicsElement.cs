using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.UI
{
    public abstract class GraphicsElement<TState>
    {
        private readonly IObservable<TState> viewStates;
        private TState currentState;

        public GraphicsElement(IObservable<TState> viewStates)
        {
            this.viewStates = viewStates;
        }

        public IObservable<TState> Show()
        {
            OnShow();
            return viewStates
                           .Do(HandleState);
        }

        public void Hide()
        {
            OnHide();
        }

        protected abstract void OnStateChange(TState oldState, TState currentState);

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        private void HandleState(TState state)
        {
            if (currentState.Equals(state))
                return;

            var oldState = currentState;
            currentState = state;

            OnStateChange(oldState, currentState);
        }


    }
}

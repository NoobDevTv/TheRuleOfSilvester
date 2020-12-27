using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;

namespace UI.Demo
{
    internal abstract class View
    {
        public Rectangle Boundry { get; set; }
        public abstract IObservable<GraphicViewState> Show();
    }

    internal abstract class View<T> : View
    {
        protected T CurrentState;
        private readonly IObservable<GraphicViewState> internalObservable;

        public View(IObservable<T> viewState, Func<IObservable<T>, IObservable<T>> createViewStates = null)
        {
            var states = createViewStates ?? CreateViewStates;
            internalObservable = states(viewState)
                                    .DistinctUntilChanged()
                                    .Do(s =>
                                    {
                                        T oldState = CurrentState;
                                        CurrentState = HandleStateChange(oldState, s);
                                    })
                                    .Select(Draw)
                                    .Select(i => new GraphicViewState(i));

        }

        protected virtual IObservable<T> CreateViewStates(IObservable<T> viewStates)
            => viewStates;

        public override IObservable<GraphicViewState> Show()
            => internalObservable;

        protected virtual T HandleStateChange(T oldState, T newState)
            => newState;

        public abstract IEnumerable<GraphicInstruction> Draw(T viewState);
    }
}

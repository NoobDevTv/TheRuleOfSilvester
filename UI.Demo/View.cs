using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Demo
{
    internal abstract class View
    {
        public Rectangle Boundry { get; set; }
        public abstract IObservable<GraphicViewState> Show();
    }

    internal abstract class View<T> : View
    {
        private T currentState;
        private readonly IObservable<GraphicViewState> internalObservable;

        public View(IObservable<T> viewState) 
            => internalObservable = viewState
                                    .DistinctUntilChanged()
                                    .Do(s =>
                                    {
                                        T oldState = currentState;
                                        currentState = HandleStateChange(oldState, s);
                                    })
                                    .Select(Draw)
                                    .Select(i => new GraphicViewState(i));

        public override IObservable<GraphicViewState> Show()
            => internalObservable;

        protected virtual T HandleStateChange(T oldState, T newState)
            => newState;

        public abstract IEnumerable<GraphicInstruction> Draw(T viewState);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Demo
{
    class View
    {
        private ViewState currentState;
        private readonly IObservable<GraphicViewState> internalObservable;

        public View(IObservable<ViewState> viewState)
        {
            internalObservable = viewState
                                    .Where(s => !string.IsNullOrWhiteSpace(s.Instructions))
                                    .DistinctUntilChanged()
                                    .Do(s =>
                                    {
                                        var oldState = currentState;
                                        currentState = s;
                                        HandleStateChange(oldState, currentState);
                                    })
                                    .Select(Draw)
                                    .Select(i => new GraphicViewState(i));
        }

        public IObservable<GraphicViewState> Show()
            => internalObservable;

        private void HandleStateChange(ViewState oldState, ViewState newState)
        {
        }

        public IEnumerable<GraphicInstruction> Draw(ViewState viewState)
        {
            yield return new GraphicInstruction.WriteLine("");
            yield return new GraphicInstruction.WriteLine("This instruction: " + viewState.Instructions);
            yield return new GraphicInstruction.WriteLine("");

        }
    }
}

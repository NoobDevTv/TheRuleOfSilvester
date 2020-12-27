using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core.SumTypes;
using UI.Demo.Inputs;

namespace UI.Demo.Views
{
    internal class TextBoxView : View<TextBoxView.TextBoxViewState>
    {
        private readonly IObservable<TextBoxViewState> focusStates;

        public TextBoxView(Router router, IObservable<TextBoxViewState> viewState) : base(viewState)
        {
            //Foucs?, Input(KeyInfo, string) -> HasFocus(bool), PressedKey(ConsoleKeyInfo), newLine(string)
            focusStates = router
                .ControlAsFocusable(this)
                .MapMany
                (
                    focus => focus.Select(f => CurrentState with { HasFocus = f.HasFocus }),
                    pressedKey => UIConsoleUtils
                                                        .StringBuilder(
                                                            pressedKey.Select(p => p.KeyInfo),
                                                            CurrentState.Text, c => true)
                                                        .Select(s => CurrentState with { Text = s }),
                    newLine => newLine.Select(l => CurrentState with { Text = l.Value })
                );
        }

        protected override IObservable<TextBoxViewState> CreateViewStates(IObservable<TextBoxViewState> viewStates)
            => Observable.Merge(viewStates, focusStates);

        protected override TextBoxViewState HandleStateChange(TextBoxViewState oldState, TextBoxViewState newState)
            => newState with { Text = newState.Text.PadRight(oldState.Text.Length) };

        public override IEnumerable<GraphicInstruction> Draw(TextBoxViewState viewState)
        {
            yield return new GraphicInstruction.Write(viewState.Text, new Point(Boundry.X, Boundry.Y));
        }

        public record TextBoxViewState(string Text, bool HasFocus);

    }
}

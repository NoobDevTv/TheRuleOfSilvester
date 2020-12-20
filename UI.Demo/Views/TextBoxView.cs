using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core.SumTypes;

namespace UI.Demo.Views
{
    internal class TextBoxView : View<TextBoxView.TextBoxViewState>
    {
        private string currentText;
        private readonly IObservable<TextBoxViewState> focusStates;

        public TextBoxView(Router router, IObservable<TextBoxViewState> viewState) : base(viewState)
        {
            //Foucs?, Input(KeyInfo, string) -> HasFocus(bool), PressedKey(ConsoleKeyInfo), newLine(string)
            focusStates = router
                .ControlAsFocusable(this)
                .MapMany
                (
                    focus => focus.Select(f => CurrentState with { HasFocus = f.HasFocus }),
                    pressedKey => Observable.Empty<TextBoxViewState>(),
                    newLine => newLine.Select(l => CurrentState with { Text = l.Value })
                );
        }

        protected override IObservable<TextBoxViewState> CreateViewStates(IObservable<TextBoxViewState> viewStates)
            => Observable.Merge(viewStates, focusStates);

        public override IEnumerable<GraphicInstruction> Draw(TextBoxViewState viewState)
        {
            yield return new GraphicInstruction.Write(viewState.Text.PadRight(currentText.Length), new Point(Boundry.X, Boundry.Y));
            currentText = viewState.Text;
        }

        public record TextBoxViewState(string Text, bool HasFocus);
       
    }
}

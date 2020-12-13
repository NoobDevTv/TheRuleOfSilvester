using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Demo.Views
{
    internal class TextBoxView : View<TextBoxView.TextBoxViewState>
    {
        private string currentText;

        public TextBoxView(Router router, IObservable<TextBoxViewState> viewState) : base(viewState)
        {
            //Foucs?, Input(KeyInfo, string) -> HasFocus(bool), PressedKey(ConsoleKeyInfo), newLine(string)
            router
                .ControlAsFocusable(this);
        }

        public override IEnumerable<GraphicInstruction> Draw(TextBoxViewState viewState)
        {
            yield return new GraphicInstruction.Write(viewState.Text.PadRight(currentText.Length), new Point(Boundry.X, Boundry.Y));
            currentText = viewState.Text;
        }

        public record TextBoxViewState(string Text);
    }
}

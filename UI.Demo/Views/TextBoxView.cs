using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Demo.Views
{
    internal class TextBoxView : View<TextBoxView.TextBoxViewState>
    {
        public TextBoxView(Router router, IObservable<TextBoxViewState> viewState) : base(viewState)
        {
            router.ControlAsFocusable(this); //Foucs?, Input(KeyInfo, string) -> HasFocus(bool), PressedKey(ConsoleKeyInfo), newLine(string)
        }

        public override IEnumerable<GraphicInstruction> Draw(TextBoxViewState viewState)
        {
            throw new NotImplementedException();
        }

        public record TextBoxViewState(string Text);
    }
}

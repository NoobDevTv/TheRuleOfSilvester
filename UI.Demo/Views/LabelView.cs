using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Demo.Views
{
    internal class LabelView : View<LabelView.LabelViewState>
    {
        public LabelView(IObservable<LabelViewState> viewState) : base(viewState)
        {
        }

        public override IEnumerable<GraphicInstruction> Draw(LabelViewState viewState)
        {
            yield return new GraphicInstruction.Write(viewState.Text, new (Boundry.X, Boundry.Y));
        }

        public record LabelViewState(string Text);
    }
}

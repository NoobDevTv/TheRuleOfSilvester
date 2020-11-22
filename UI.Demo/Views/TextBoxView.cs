using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Demo.Views
{
    internal class TextBoxView : View
    {
        public TextBoxView(IObservable<ViewState> viewState) : base(viewState)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Demo.Views
{
    internal class GridView : View
    {
        public GridView(IObservable<ViewState> viewState) : base(viewState)
        {
        }
    }
}

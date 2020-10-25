using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.UI.Views;

namespace TheRuleOfSilvester.UI
{
    public class Router : IDisposable
    {
        public RouterOutlet CurrentOutlet { get;  }

        public Router()
        {
            CurrentOutlet = new RouterOutlet();
        }

        public void Dispose()
        {
            CurrentOutlet.Dispose();
        }

        public void Navigate(View view)
        {
        }
    }
}

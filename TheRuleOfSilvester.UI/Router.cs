using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal void ShowOutlet()
        {
        }
    }
}

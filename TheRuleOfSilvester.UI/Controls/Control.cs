using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.UI.Controls
{
    public abstract class Control : GraphicsElement<ControlState>
    {
        public Control(IObservable<ControlState> controlStates) 
            : base(controlStates)
        {

        }      
    }
}

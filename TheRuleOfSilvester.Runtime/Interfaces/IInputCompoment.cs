using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface IInputCompoment
    {
        IObservable<InputAction> InputActions { get; }

    }
}

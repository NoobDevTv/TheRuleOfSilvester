using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface IInputComponent
    {
        IObservable<InputAction> InputActions { get; }

    }
}

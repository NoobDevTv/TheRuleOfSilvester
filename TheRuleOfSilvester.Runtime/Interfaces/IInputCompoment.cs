using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface IInputCompoment : IDisposable
    {
        ConcurrentQueue<InputAction> InputActions { get; }

        bool Active { get; set; }
    }
}

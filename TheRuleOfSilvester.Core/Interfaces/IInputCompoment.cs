using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Interfaces
{
    public interface IInputCompoment
    {
        ConcurrentQueue<InputAction> InputActions { get; }

        bool Active { get; set; }
    }
}

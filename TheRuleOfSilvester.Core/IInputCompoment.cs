using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public interface IInputCompoment
    {
        bool Up { get; }
        bool Down { get; }
        bool Left { get; }
        bool Right { get; }
        bool StartAction { get; }

        int LastKey { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public class InputAction
    {
        public InputActionType Type { get; private set; }
        public bool Valid { get; private set; }

        public InputAction(InputActionType actionType)
        {
            Type = actionType;
            Valid = true;
        }
        public InputAction(int actionType) : this((InputActionType)actionType)
        {
        }

        internal void SetInvalid()
        {
            Valid = false;
        }
    }

}

using System;

namespace TheRuleOfSilvester.Core
{
    public delegate void PropertyChangeEventHandler(object sender, PropertyChangeEventArgs e);

    public class PropertyChangeEventArgs : EventArgs
    {
        public string PropertyName { get; }
        public object OldValue { get; }
        public object NewValue { get; }

        public PropertyChangeEventArgs(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
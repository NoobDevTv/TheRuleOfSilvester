using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core.Observation;

namespace TheRuleOfSilvester.Server
{
    public abstract class CommandObserver : INotificationObserver<CommandNotification>
    {
        public virtual void OnCompleted()
        {
        }

        public virtual void OnError(Exception error)
        {
        }

        public virtual object OnNext(CommandNotification value)
        {
            return default;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Observation
{
    public sealed class Notification : INotification
    {
        public NotificationType Type { get; }
        
        private readonly byte[] data;

        public Notification(byte[] data, NotificationType type)
        {
            this.data = data;
            Type = type;
        }

        public T Deserialize<T>(Func<byte[], T> deserializeMethod)
            => deserializeMethod(data);
    }
}

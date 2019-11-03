using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheRuleOfSilvester.Core.Observation
{
    public sealed class Notification : INotification
    {
        public static Notification Empty { get; set; }

        static Notification()
        {
            Empty = new Notification(Array.Empty<byte>(), NotificationType.Empty);
        }

        public NotificationType Type { get; }

        private readonly byte[] data;

        public Notification(byte[] data, NotificationType type)
        {
            this.data = data;
            Type = type;
        }

        public T Deserialize<T>(Func<byte[], T> deserializeMethod)
            => deserializeMethod(data);

        public byte[] Serialize()
            => BitConverter.GetBytes((int)Type).Concat(data).ToArray();
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace TheRuleOfSilvester.Core.Roles
{
    public abstract class BaseRole : IByteSerializable
    {
        public string Name { get; private set; }

        public List<Goal> Goals { get; set; }

        public int ActionsPoints { get; protected set; }
        public int HealthPoints
        {
            get => healthPoints;
            internal set => healthPoints = value > MaxHealthPoints ? MaxHealthPoints : value;
        }

        public int MaxHealthPoints { get; protected set; }
        public int Attack { get; protected set; }
        public int Defence { get; protected set; }
        public bool RedrawStats { get; set; }
        public char Avatar { get; protected set; }

        private int healthPoints;

        protected BaseRole(string name)
        {
            Name = name;
            RedrawStats = true;
        }

        public void Serialize(BinaryWriter binaryWriter)
            => binaryWriter.Write(GetType().FullName);

        public void Deserialize(BinaryReader binaryReader)
            => throw new NotSupportedException("see player");
    }
}

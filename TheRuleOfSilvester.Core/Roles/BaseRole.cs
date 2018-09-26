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
        public int HealthPoints { get; protected set; }
        public int Attack { get; protected set; }
        public int Defence { get; protected set; }

        protected BaseRole(string name) 
            => Name = name;

        public void Serialize(BinaryWriter binaryWriter)
            => binaryWriter.Write(GetType().FullName);

        public void Deserialize(BinaryReader binaryReader)
            => throw new NotSupportedException("see player");
    }
}

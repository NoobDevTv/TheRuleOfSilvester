using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.Items
{
    public class BaseItemCell : Cell, IByteSerializable
    {
        public Guid Guid { get; private set; }

        public BaseItemCell(Map map, bool movable = true) : base(1, 1, map, movable)
        {
            var guidVal = GetType().GetCustomAttribute<GuidAttribute>()?.Value;
            Guid = string.IsNullOrWhiteSpace(guidVal) ? Guid.NewGuid() : new Guid(guidVal);
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Guid.ToByteArray());
            binaryWriter.Write(Movable);
            binaryWriter.Write(Position.X);
            binaryWriter.Write(Position.Y);
            binaryWriter.Write(Color.ToArgb());
        }

        public void Deserialize(BinaryReader binaryReader)
            => throw new NotSupportedException("Use SerializeHelper.DeserializeMapCell instead ;)");

    }
}

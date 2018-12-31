using System.IO;

namespace TheRuleOfSilvester.Core.Interfaces
{
    public interface IByteSerializable
    {
        void Serialize(BinaryWriter binaryWriter);
        void Deserialize(BinaryReader binaryReader);
    }
}
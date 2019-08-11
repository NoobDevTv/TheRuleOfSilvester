using System.IO;

namespace TheRuleOfSilvester.Core
{
    public interface IByteSerializable
    {
        void Serialize(BinaryWriter binaryWriter);
        void Deserialize(BinaryReader binaryReader);
    }
}
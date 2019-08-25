using System.IO;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime
{
    public interface IPlayerAction
    {
        ActionType ActionType { get; }
        uint Order { get; set; }
        IPlayer Player { get; set; }
        Position Point { get; }

        void Deserialize(BinaryReader binaryReader);
        void Serialize(BinaryWriter binaryWriter);
        string ToString();
    }
}
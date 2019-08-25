using System.Collections.Generic;
using System.IO;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Core
{
    public interface IBaseRole
    {
        int ActionsPoints { get; }
        int Attack { get; }
        char Avatar { get; }
        List<ICondition> Conditions { get; set; }
        int Defence { get; }
        int HealthPoints { get; }
        int MaxHealthPoints { get; }
        string Name { get; }
        bool RedrawStats { get; set; }
        int RestActionPoints { get; }

        void Deserialize(BinaryReader binaryReader);
        void ResetActionPoints();
        void Serialize(BinaryWriter binaryWriter);
        void SetUsedActionPoints(int actionPoints);
        void UseActionPoint(int actionPoints = 1);
    }
}
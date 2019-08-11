using System;
using System.Collections.Generic;
using System.IO;
using TheRuleOfSilvester.Runtime.Conditions;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Runtime.Items;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.Roles
{
    public abstract class BaseRole : IByteSerializable
    {
        public string Name { get; private set; }

        public List<ICondition> Conditions { get; set; }

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
        public abstract char Avatar { get; }
        public int RestActionPoints => ActionsPoints;//- currentActionPoints;

        private int healthPoints;
        private int currentActionPoints;

        protected BaseRole(string name)
        {
            currentActionPoints = 0;
            Name = name;
            RedrawStats = true;
            Conditions = new List<ICondition>() { new ItemHoldCondition() { ItemType = typeof(CoinStack) } };
        }

        public void Serialize(BinaryWriter binaryWriter)
            => binaryWriter.Write(GetType().FullName);

        public void Deserialize(BinaryReader binaryReader)
            => throw new NotSupportedException("see player");

        public void ResetActionPoints()
        {
            currentActionPoints = 0;
        }

        public void UseActionPoint(int actionPoints = 1)
        {
            currentActionPoints += actionPoints;
        }

        public void SetUsedActionPoints(int actionPoints)
        {
            currentActionPoints = actionPoints;
            RedrawStats = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Roles
{
    public class Warrior : BaseRole
    {
        public override char Avatar => '♞';
        public Warrior() : base(nameof(Warrior))
        {
            HealthPoints = 120;
            MaxHealthPoints = HealthPoints;
            ActionsPoints = 5;
            Attack = 20;
            Defence = 10;
        }
    }
}

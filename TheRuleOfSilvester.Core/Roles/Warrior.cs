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
            MaxHealthPoints = 120;
            HealthPoints = MaxHealthPoints;
            ActionsPoints = 5;
            Attack = 20;
            Defence = 10;
            Avatar = '♞';
        }
    }
}

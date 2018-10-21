using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Roles
{
    public class Mage : BaseRole
    {
        public override char Avatar => '⛤';
        public Mage() : base(nameof(Mage))
        {
            HealthPoints = 100;
            MaxHealthPoints = HealthPoints;
            ActionsPoints = 8;
            Attack = 10;
            Defence = 2;
        }
    }
}

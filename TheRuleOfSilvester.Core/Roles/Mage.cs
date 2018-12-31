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
            MaxHealthPoints = 100;
            HealthPoints = MaxHealthPoints;
            ActionsPoints = 7;
            Attack = 10;
            Defence = 2;
        }
    }
}

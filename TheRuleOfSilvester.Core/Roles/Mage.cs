using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Roles
{
    public class Mage : BaseRole
    {
        public Mage() : base(nameof(Mage))
        {
            MaxHealthPoints = 100;
            HealthPoints = MaxHealthPoints;
            ActionsPoints = 8;
            Attack = 10;
            Defence = 2;
            Avatar = '⛤';
        }
    }
}

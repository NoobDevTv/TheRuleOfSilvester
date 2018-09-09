using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Roles
{
    public class Mage : BaseRole
    {
        public Mage() : base(nameof(Mage))
        {
            HealthPoints = 100;
            ActionsPoints = 8;
            Attack = 10;
            Defence = 2;
        }
    }
}

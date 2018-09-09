using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Roles
{
    public class Warrior : BaseRole
    {
        public Warrior() : base(nameof(Warrior))
        {
            HealthPoints = 120;
            ActionsPoints = 5;
            Attack = 20;
            Defence = 10;
        }
    }
}

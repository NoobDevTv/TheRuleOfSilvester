using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Roles
{
    public class Thief : BaseRole
    {
        public Thief() : base(nameof(Thief))
        {
            HealthPoints = 70;
            MaxHealthPoints = HealthPoints;
            ActionsPoints = 13;
            Attack = 15;
            Defence = 5;
        }
    }
}

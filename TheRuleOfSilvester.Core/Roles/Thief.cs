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
            ActionsPoints = 13;
            Attack = 15;
            Defence = 5;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Roles
{
    public class Assassin : BaseRole
    {
        public Assassin() : base(nameof(Assassin))
        {
            HealthPoints = 50;
            ActionsPoints = 30;
            Attack = 30;
            Defence = 0;
        }
    }
}

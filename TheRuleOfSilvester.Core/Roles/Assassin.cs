using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Roles
{
    public class Assassin : BaseRole
    {
        public override char Avatar => '➳';

        public Assassin() : base(nameof(Assassin))
        {
            MaxHealthPoints = 50;
            HealthPoints = MaxHealthPoints;
            ActionsPoints = 30;
            Attack = 30;
            Defence = 0;
        }
    }
}

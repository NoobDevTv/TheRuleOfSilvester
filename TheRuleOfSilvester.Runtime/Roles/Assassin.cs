using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Runtime.Roles
{
    public class Assassin : BaseRole
    {
        public override char Avatar => '➳';

        public Assassin() : base(nameof(Assassin))
        {
            MaxHealthPoints = 50;
            HealthPoints = MaxHealthPoints;
            ActionsPoints = 10;
            Attack = 30;
            Defence = 0;
        }
    }
}

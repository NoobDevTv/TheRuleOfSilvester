using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Roles
{
    public class Thief : BaseRole
    {
        public override char Avatar => '✋';
        public Thief() : base(nameof(Thief))
        {
            MaxHealthPoints = 70;
            HealthPoints = MaxHealthPoints;
            ActionsPoints = 13;
            Attack = 15;
            Defence = 5;
            Avatar = '✋';
        }
    }
}

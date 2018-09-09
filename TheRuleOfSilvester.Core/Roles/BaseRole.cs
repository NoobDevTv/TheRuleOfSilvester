using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Roles
{
    public abstract class BaseRole
    {
        public string Name { get; private set; }

        public List<Goal> Goals { get; set; }

        public int ActionsPoints { get; protected set; }
        public int HealthPoints { get; protected set; }
        public int Attack { get; protected set; }
        public int Defence { get; protected set; }

        protected BaseRole(string name) => Name = name;
    }
}

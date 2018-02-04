using System;
using System.Collections.Generic;

namespace TheRuleOfSilvester.Core
{
    public abstract class BasicMapGenerator
    {
        protected Random random;
        public List<Type> CellTypes { get; protected set; }

        public BasicMapGenerator()
        {
            random = new Random();
        }

        public abstract Map Generate(int x, int y);
    }
}
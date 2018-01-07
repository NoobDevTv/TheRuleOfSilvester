using System;

namespace TheRuleOfSilvester.Core
{
    public abstract class BasicMapGenerator
    {
        protected Random random;

        public BasicMapGenerator()
        {
            random = new Random();
        }

        public abstract Map Generate(int x, int y);
    }
}
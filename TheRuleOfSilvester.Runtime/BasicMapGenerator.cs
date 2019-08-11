using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TheRuleOfSilvester.Runtime.Cells;

namespace TheRuleOfSilvester.Runtime
{
    public abstract class BasicMapGenerator
    {
        protected Random random;
        public List<Type> CellTypes { get; protected set; }

        public BasicMapGenerator()
        {
            random = new Random();
        }

        public BasicMapGenerator(Guid[] cellGUIDs) : this()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(c => c.BaseType == typeof(MapCell)).ToList();

            CellTypes = new List<Type>();

            foreach (var item in cellGUIDs)
                CellTypes.Add(types.FirstOrDefault(c => c.GUID == item));
        }

        public abstract Map Generate(int x, int y);
    }
}
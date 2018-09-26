using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheRuleOfSilvester.Core.Roles
{
    public static class RoleManager
    {
        private static List<Type> baseRoles;

        static RoleManager()
        {
            baseRoles = new List<Type>();

            baseRoles.AddRange(Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(BaseRole)));
        }

        public static Queue<BaseRole> GetAllRolesRandomized()
        {
            Random r = new Random();
            return new Queue<BaseRole>(baseRoles.OrderBy(x => r.Next()).Select(x => (BaseRole)Activator.CreateInstance(x)));
        }

        public static Queue<BaseRole> GetRandomRoles(int amount)
        {
            Random r = new Random();
            var queue = new Queue<BaseRole>();

            for (int i = 0; i < amount; i++)
                queue.Enqueue((BaseRole)Activator.CreateInstance(baseRoles[r.Next(0, baseRoles.Count)]));

            return queue;
        }

        public static BaseRole GetRandomRole()
        {
            Random r = new Random();
            return (BaseRole)Activator.CreateInstance(baseRoles[r.Next(0, baseRoles.Count)]);
        }
    }
}

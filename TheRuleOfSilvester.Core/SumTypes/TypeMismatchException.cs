
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Core.SumTypes
{
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException(): base($"Unexpected Type")
        {
            //var demo = new Variant<string, string, int, int>();
            //demo.TestMethod();

            var test = new Variant<int, string>("");
            test.Map(i => i += 1, s => Console.WriteLine(s));

        }

    }
    
}

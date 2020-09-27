
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
    

        }

    }
    
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core.SumTypes;

namespace UI.Demo
{
    public sealed class ConsoleState : Variant<string, Point>
    {
        public ConsoleState(string value) : base(value)
        {
        }
        public ConsoleState(Point value) : base(value)
        {
        }

        public static implicit operator ConsoleState(string value)
           => new ConsoleState(value);
        public static implicit operator ConsoleState(Point value)
            => new ConsoleState(value);
    }
}

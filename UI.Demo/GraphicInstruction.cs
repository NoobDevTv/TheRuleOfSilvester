using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core.SumTypes;

namespace UI.Demo
{
    public class GraphicInstruction : Variant<GraphicInstruction.WriteLine, GraphicInstruction.Write, GraphicInstruction.SetPosition>
    {
        public GraphicInstruction(WriteLine value) : base(value)
        {
        }
        public GraphicInstruction(Write value) : base(value)
        {
        }
        public GraphicInstruction(SetPosition value) : base(value)
        {
        }

        public readonly struct WriteLine
        {
            public string Value { get; }

            public WriteLine(string value) 
                => Value = value;
        }

        public readonly struct Write
        {
            public string Value { get; }

            public Write(string value)
                => Value = value;
        }

        public readonly struct SetPosition
        {
            public Point Value { get; }

            public SetPosition(Point value)
                => Value = value;
        }

        public static implicit operator GraphicInstruction(WriteLine value)
            => new GraphicInstruction(value);
        public static implicit operator GraphicInstruction(Write value)
            => new GraphicInstruction(value);
        public static implicit operator GraphicInstruction(SetPosition value)
            => new GraphicInstruction(value);
    }
}

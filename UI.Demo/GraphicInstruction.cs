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

        public record WriteLine (string Value, Point Position) : SetPosition(Position);

        public record Write(string Value, Point Position) : SetPosition(Position);

        public record SetPosition(Point Position);

        public static implicit operator GraphicInstruction(WriteLine value)
            => new(value);
        public static implicit operator GraphicInstruction(Write value)
            => new(value);
        public static implicit operator GraphicInstruction(SetPosition value)
            => new(value);
    }
}

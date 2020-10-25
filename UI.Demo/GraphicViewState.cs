using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Demo
{
    public sealed class GraphicViewState
    {
        public IEnumerable<GraphicInstruction> Instructions { get;  }

        public GraphicViewState(IEnumerable<GraphicInstruction> instructions) => Instructions = instructions;
    }
}

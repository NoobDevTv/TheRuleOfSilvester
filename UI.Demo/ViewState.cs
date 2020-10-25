using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Demo
{
    public readonly struct ViewState : IEquatable<ViewState>
    {
        public readonly string Instructions { get;  }

        public ViewState(string instructions) 
            => Instructions = instructions;

        public override bool Equals(object obj) => obj is ViewState state && Equals(state);
        public bool Equals(ViewState other) => Instructions == other.Instructions;
        public override int GetHashCode() => HashCode.Combine(Instructions);

        public static bool operator ==(ViewState left, ViewState right) => left.Equals(right);
        public static bool operator !=(ViewState left, ViewState right) => !(left == right);
    }
}

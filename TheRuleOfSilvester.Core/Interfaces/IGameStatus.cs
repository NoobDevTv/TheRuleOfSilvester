using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Interfaces
{
    public interface IGameStatus
    {
        GameStatus CurrentGameStatus { get; }
    }
}

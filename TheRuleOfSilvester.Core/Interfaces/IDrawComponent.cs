using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Interfaces
{
    public interface IDrawComponent
    {
        void Draw(Map map);
        void DrawCells<T>(List<T> cells) where T : Cell;
    }
}

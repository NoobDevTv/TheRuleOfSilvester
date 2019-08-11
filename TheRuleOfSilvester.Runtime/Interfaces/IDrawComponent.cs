using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Runtime.Interfaces
{
    public interface IDrawComponent
    {
        void Draw(Map map);
        void DrawTextCells(List<TextCell> cells);
    }
}

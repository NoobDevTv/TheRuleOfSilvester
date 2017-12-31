using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester
{
    internal class DrawComponent : IDrawComponent
    {
        public void Draw(Map map)
        {
            for (int i = 0; i < map.Cells.GetLength(0); i++)
            {
                for (int o = 0; o < map.Cells.GetLength(1); o++)
                {
                    var cell = map.Cells[i, o];
                    if (cell != null && cell.Invalid)
                    {
                        for (int k = 0; k < cell.Lines.GetLength(0); k++)
                        {
                            for (int l = 0; l < cell.Lines.GetLength(1); l++)
                            {
                                Console.SetCursorPosition(o * 5 + l, i * 3 + k);
                                if (cell.Lines[k, l] == null)
                                    Console.Write(" ");
                                else
                                    Console.Write(cell.Lines[k, l]);
                                cell.Invalid = false;
                            }
                        }
                    }
                }
            }
        }
    }
}

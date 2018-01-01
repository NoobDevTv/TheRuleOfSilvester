using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester
{
    internal class DrawComponent : IDrawComponent
    {
        public void Draw(Map map)
        {
            foreach (var cell in map.Cells)
            {
                if (cell.Invalid)
                {
                    for (int k = 0; k < cell.Lines.GetLength(0); k++)
                    {
                        for (int l = 0; l < cell.Lines.GetLength(1); l++)
                        {
                            Console.SetCursorPosition(cell.Position.Y * cell.Width + l, cell.Position.X * cell.Height + k);

                            if (cell.Lines[k, l] == null)
                                Console.Write(" ");
                            else
                                Console.Write(cell.Lines[k, l]);

                            cell.Invalid = false;
                        }

                    }
                }
            }
            //TODO: Quick and Dirty, must be set to player pos later on
            //TODO: Unschön
            Console.SetCursorPosition(110, 20);
        }
    }
}

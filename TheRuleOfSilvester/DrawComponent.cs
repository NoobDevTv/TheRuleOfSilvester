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
            DrawCells(map.Cells);

            //TODO: Quick and Dirty, must be set to player pos later on
            //TODO: Unschön
            DrawCells(((Player)map.Cells.FirstOrDefault(x => x.GetType() == typeof(Player))).Inventory);
            Console.SetCursorPosition(map.Width*5+10, map.Height * 3+ 10);

        }
        private void DrawCells(List<Cell> cells)
        {
            foreach (var cell in cells)
            {
                if (cell.Invalid)
                {
                    if (Enum.TryParse(cell.Color.Name, out ConsoleColor color))
                        Console.ForegroundColor = color;
                    else
                        Console.ForegroundColor = ConsoleColor.White;

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

        }
    }
}

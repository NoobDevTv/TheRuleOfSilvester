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
            DrawCells(map.Players);

            //TODO: Unschön, Spieler muss wissen wer er ist
            DrawCells(map.Players.FirstOrDefault().Inventory);

            DrawCells(map.TextCells);

            Console.SetCursorPosition(Console.WindowWidth - 2, Console.WindowHeight - 2);
        }

        private void DrawCells<T>(List<T> cells) where T : Cell
        {
            foreach (var cell in cells)
            {
                if (cell.Invalid)
                {
                    if (Enum.TryParse(cell.Color.Name, out ConsoleColor color))
                        Console.ForegroundColor = color;
                    else
                        Console.ForegroundColor = ConsoleColor.White;

                    for (int l = 0; l < cell.Lines.GetLength(0); l++)
                    {
                        for (int h = 0; h < cell.Lines.GetLength(1); h++)
                        {
                            Console.SetCursorPosition(cell.Position.X * cell.Width + l, cell.Position.Y * cell.Height + h);

                            Console.Write(cell.Lines[l, h]);

                            cell.Invalid = false;
                        }
                    }
                }
            }

        }
    }
}

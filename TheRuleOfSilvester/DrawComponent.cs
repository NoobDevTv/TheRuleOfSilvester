using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester
{
    internal class DrawComponent : IDrawComponent
    {

        public void Draw(Map map)
        {
            DrawCells(map.Cells);
            
            //TODO: Quick and Dirty, must be set to player pos later on
            DrawCells(map.Players);

            //TODO: Unschön, Spieler weiß wer er ist, vlt. anders schöner?
            DrawCells(map.Players.FirstOrDefault(x=>x.IsLocal).Inventory);

            DrawCells(map.TextCells);

            Console.SetCursorPosition(Console.WindowWidth - 2, Console.WindowHeight - 2);
        }

        private void DrawCells<T>(List<T> cells) where T : Cell
        {
            foreach (var cell in cells.ToArray())
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

                            if (cell.Layer[l, h] != null)
                                Console.Write(BaseElementToChar(cell.Layer[l, h]));
                            else
                                Console.Write(BaseElementToChar(cell.Lines[l, h]));

                            cell.Invalid = false;
                        }
                    }
                }
            }

        }

        private char BaseElementToChar(BaseElement baseElement)
        {
            //TODO implement Overlayering
            if (baseElement == null)
                return ' ';
            switch (baseElement.ElementID)
            {
                case 1: return '│';
                case 2: return '║';
                case 3: return '─';
                case 4: return '═';
                case 5: return '┌';
                case 6: return '╔';
                case 7: return '└';
                case 8: return '╚';
                case 9: return '┐';
                case 10: return '╗';
                case 11: return '┘';
                case 12: return '╝';
                case 13: return '┬';
                case 14: return '╦';
                case 15: return '┴';
                case 16: return '╩';
                case 17: return '├';
                case 18: return '╠';
                case 19: return '┤';
                case 20: return '╣';
                case 21: return '┼';
                case 22: return '╬';
                default:
                    return (char)baseElement.ElementID;
            }
        }
    }
}

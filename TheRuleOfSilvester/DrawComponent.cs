using System;
using System.Collections.Generic;
using System.Linq;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Cells;
using TheRuleOfSilvester.Core.Roles;

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
            DrawCells(map.Players.FirstOrDefault(x => x.IsLocal).Inventory);

            DrawCells(map.TextCells);
            DrawPlayerInfo(map.Players.FirstOrDefault(x => x.IsLocal), map);

            Console.SetCursorPosition(Console.WindowWidth - 2, Console.WindowHeight - 2);
        }

        private void DrawPlayerHealth(BaseRole role)
        {
            string s = "";

            var points = role.HealthPoints * 10d / role.MaxHealthPoints;

            for (int i = 1; i < points; points--)
                s += "█";
            Console.Write(s);//U+2588

            char c = '█';
            c += (char)Math.Ceiling(7 - points * 7);
            Console.Write(c);

            for (int i = 0; i < 10 - role.HealthPoints * 10 / role.MaxHealthPoints; i++)
                Console.Write(" ");
        }

        public void DrawPlayerInfo(Player player, Map map)
        {
            if (!player.Role.RedrawStats)
                return;
            int topPos = 1;
            void ResetCursor()
            {
                Console.CursorLeft = Console.WindowWidth - 26;
                Console.CursorTop = topPos++;
            }
            ResetCursor();
            Console.Write(player.Name);
            ResetCursor();
            Console.Write($"♥HP: {player.Role.HealthPoints}   ");
            ResetCursor();
            Console.ForegroundColor = ConsoleColor.Red;
            DrawPlayerHealth(player.Role);
            Console.ForegroundColor = ConsoleColor.White;
            ResetCursor();
            Console.Write("⚔ATK: " + player.Role.Attack);
            ResetCursor();
            Console.Write("⛨DEF: " + player.Role.Defence);
            player.Role.RedrawStats = false;
        }

        public void DrawCells<T>(List<T> cells) where T : Cell
        {
            foreach (var cell in cells.ToArray())
            {
                if (cell.Invalid)
                {
                    Console.ForegroundColor = Enum.TryParse(cell.Color.Name, out ConsoleColor color) ? color : ConsoleColor.White;

                    for (int l = 0; l < cell.Lines.GetLength(0); l++)
                    {
                        for (int h = 0; h < cell.Lines.GetLength(1); h++)
                        {
                            Console.SetCursorPosition((cell.Position.X * cell.Width) + l, (cell.Position.Y * cell.Height) + h);

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

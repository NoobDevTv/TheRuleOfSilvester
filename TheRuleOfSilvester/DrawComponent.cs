using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Cells;
using TheRuleOfSilvester.Core.Interfaces;
using TheRuleOfSilvester.Core.Items;
using TheRuleOfSilvester.Core.Roles;

namespace TheRuleOfSilvester
{
    internal class DrawComponent : IDrawComponent
    {
        public const int INFO_WIDTH = 23;
        public const int INVENTORY_HEIGHT = 8;

        public int CurrentWidth => Console.WindowWidth - INFO_WIDTH;
        public int CurrentHeight => Console.WindowHeight - INVENTORY_HEIGHT;

        private bool chunkChange;
        private Point oldChunkPos;
        private int oldWidth = 0;
        private int oldHeight = 0;
        private BaseItemCell[] oldPlayerInventory;
        private readonly ChunkCollection chunks;

        public DrawComponent()
        {
            oldWidth = 0;
            oldHeight = 0;
            chunks = new ChunkCollection();
            chunkChange = false;
            oldChunkPos = new Point(0, 0);
        }

        public void Draw(Map map)
        {
            if (oldWidth != CurrentWidth || oldHeight != CurrentHeight)
                RecalculateChunks(map);
            Console.CursorVisible = false;
            //TODO: Unschön, Spieler weiß wer er ist, vlt. anders schöner?
            var localPlayer = map.Players.FirstOrDefault(x => x.IsLocal);
            
            var chunkPosX = (localPlayer.Position.X-1) / CurrentWidth;
            var chunkPosY = (localPlayer.Position.Y-1) / CurrentHeight;

            if (oldChunkPos.X != chunkPosX || oldChunkPos.Y != chunkPosY)
            {
                chunkChange = true;
                oldChunkPos = new Point(chunkPosX, chunkPosY);
            }

            if (chunkChange)
                Console.Clear();

            //DrawCells(map.Cells, chunks[chunkPosX, chunkPosY]);
            ////TODO: Quick and Dirty, must be set to player pos later on
            DrawCells(map.Players, chunks[chunkPosX, chunkPosY]);

            DrawCells(localPlayer.CellInventory);

            DrawCells(map.TextCells);
            DrawPlayerInfo(localPlayer);
            DrawItemInventory(localPlayer);
            chunkChange = false;
            Console.SetCursorPosition(Console.WindowWidth - 2, Console.WindowHeight - 2);
        }

        private void RecalculateChunks(Map map)
        {
            oldWidth = CurrentWidth;
            oldHeight = CurrentHeight;

            chunks.Clear();

            var referenceCell = map.Cells.First();
            for (int w = 0; w < Math.Ceiling(map.Width * referenceCell.Width / (float)CurrentWidth); w++)
                for (int h = 0; h < Math.Ceiling(map.Height * referenceCell.Height / (float)CurrentHeight); h++)
                    chunks.Add(new Chunk(map.Cells, CurrentWidth - 8, CurrentHeight, new Point(w, h)));

            chunkChange = true;
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

        public void DrawPlayerInfo(Player player)
        {
            if (!player.Role.RedrawStats && !chunkChange)
                return;
            int topPos = 1;
            void ResetCursor()
            {
                Console.CursorLeft = CurrentWidth;
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
            ResetCursor();
            Console.Write("▼AP: " + player.Role.RestActionPoints);
            player.Role.RedrawStats = false;
        }

        public void DrawItemInventory(Player player)
        {
            int topPos = 7;
            void ResetCursor()
            {
                Console.CursorLeft = Console.WindowWidth - INFO_WIDTH;
                Console.CursorTop = topPos++;
            }

            var itemInventory = player.ItemInventory;

            if (oldPlayerInventory == null)
                oldPlayerInventory = new BaseItemCell[itemInventory.Length];

            for (int i = 0; i < itemInventory.Length; i++)
            {
                ResetCursor();

                if (itemInventory[i] != oldPlayerInventory[i] || chunkChange)
                {
                    if (itemInventory[i] == null)
                        Console.Write(' ');
                    else
                        Console.Write((char)itemInventory[i].Lines[0, 0].ElementID);
                }
            }

            player.ItemInventory.CopyTo(oldPlayerInventory, 0);
        }

        public void DrawCells<T>(List<T> cells) where T : Cell
        {
            foreach (var cell in cells.ToArray())
            {
                if (cell.Invalid || chunkChange)
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

        public void DrawCells<T>(List<T> cells, Chunk chunk) where T : Cell
        {
            foreach (var cell in chunk.Cells.Concat(cells))
            {
                if (cell.Invalid || chunkChange)
                {
                    Console.ForegroundColor = Enum.TryParse(cell.Color.Name, out ConsoleColor color) ? color : ConsoleColor.White;

                    for (int l = 0; l < cell.Lines.GetLength(0); l++)
                    {
                        for (int h = 0; h < cell.Lines.GetLength(1); h++)
                        {
                            Console.SetCursorPosition(cell.AbsolutPosition.X - (chunk.ChunkPosition.X * CurrentWidth) + l, cell.AbsolutPosition.Y - (chunk.ChunkPosition.Y * CurrentHeight) + h);

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

        private class ChunkCollection : List<Chunk>
        {
            public Chunk this[int x, int y]
                => this.First(c => c.ChunkPosition.X == x && c.ChunkPosition.Y == y);
        }
    }
}

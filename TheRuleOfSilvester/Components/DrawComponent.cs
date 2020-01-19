using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Runtime;
using TheRuleOfSilvester.Runtime.Cells;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Runtime.Items;
using TheRuleOfSilvester.Runtime.Roles;

namespace TheRuleOfSilvester.Components
{
    internal class DrawComponent : IDrawComponent
    {
        public const int INFO_WIDTH = 23;
        public const int INVENTORY_HEIGHT = 8;

        public int CurrentWidth { get; set; }
        public int CurrentHeight { get; set; }

        private bool chunkChange;
        private Position oldChunkPos;
        private int oldWidth = 0;
        private int oldHeight = 0;
        private BaseItemCell[] oldPlayerInventory;
        private readonly ChunkCollection chunks;

        public DrawComponent()
        {
            oldWidth = 0;
            oldHeight = 0;
            CurrentWidth = 0;
            CurrentHeight = 0;
            chunks = new ChunkCollection();
            chunkChange = false;
            oldChunkPos = new Position(0, 0);
        }

        public void Draw(Map map)
        {
            if (oldWidth != Console.WindowWidth || oldHeight != Console.WindowHeight)
            {
                CurrentWidth = Console.WindowWidth - INFO_WIDTH;
                CurrentHeight = Console.WindowHeight - INVENTORY_HEIGHT;

                RecalculateChunks(map);

                oldWidth = Console.WindowWidth;
                oldHeight = Console.WindowHeight;
            }
            Console.CursorVisible = false;
            //TODO: Unschön, Spieler weiß wer er ist, vlt. anders schöner?

            var localGhostPlayer = map.Players.OfType<GhostPlayer>().FirstOrDefault(x => x.IsLocal);
            var localPlayer = map.Players.OfType<Player>().FirstOrDefault(x => x.IsLocal);
            int chunkPosX = 0;
            int chunkPosY = 0;
            if (localGhostPlayer != null)
            {
                chunkPosX = (localGhostPlayer.Position.X - 1) / (CurrentWidth - 8);
                chunkPosY = (localGhostPlayer.Position.Y - 1) / CurrentHeight;
            }
            else
            {
                chunkPosX = (localPlayer.Position.X - 1) / (CurrentWidth - 8);
                chunkPosY = (localPlayer.Position.Y - 1) / CurrentHeight;
            }

            if (oldChunkPos.X != chunkPosX || oldChunkPos.Y != chunkPosY)
            {
                chunkChange = true;
                oldChunkPos = new Position(chunkPosX, chunkPosY);
            }

            if (chunkChange)
                Console.Clear();

            //DrawCells(map.Cells, chunks[chunkPosX, chunkPosY]);
            ////TODO: Quick and Dirty, must be set to player pos later on
            try
            {
                DrawCells(map.Players, chunks[chunkPosX, chunkPosY]);
                DrawTextCells(map.TextCells);
                DrawPlayerInfo(localPlayer);
                DrawItemInventory(localPlayer);
                DrawCellInventory(localPlayer.CellInventory);
            }
            catch (Exception)
            {
            }
            chunkChange = false;
            Console.SetCursorPosition(Console.WindowWidth - 2, Console.WindowHeight - 2);
        }



        public void DrawTextCells(List<TextCell> cells)
        {
            int offset;
            foreach (var cellGroup in cells.GroupBy(x => x?.Position?.X))
            {
                offset = 1;
                foreach (var cell in cellGroup)
                {
                    if (cell.Invalid || chunkChange)
                    {
                        Console.ForegroundColor = Enum.TryParse(cell.Color.Name, out ConsoleColor color) ? color : ConsoleColor.White;

                        for (int l = 0; l < cell.Width; l++)
                        {
                            for (int h = 0; h < cell.Height; h++)
                            {
                                Console.SetCursorPosition((cell.Position.X * cell.Width) + l, CurrentHeight + h + offset);

                                if (cell.Layer[l, h] != null)
                                    Console.Write(BaseElementToChar(cell.Layer[l, h]));
                                else
                                    Console.Write(BaseElementToChar(cell.Lines[l, h]));

                                cell.Invalid = false;
                            }
                        }
                    }
                    offset += cell.Height;
                }
            }

        }

        private void DrawCells<T>(List<T> cells, Chunk chunk) where T : Cell
        {
            foreach (var cell in chunk?.Cells?.Concat(cells))
            {
                if (cell.Invalid || chunkChange)
                {
                    Console.ForegroundColor = Enum.TryParse(cell.Color.Name, out ConsoleColor color) ? color : ConsoleColor.White;

                    if (cell is MapCell mapCell)
                        mapCell.NormalizeLayering();

                    for (int w = 0; w < cell.Width; w++)
                    {
                        for (int h = 0; h < cell.Height; h++)
                        {
                            if (cell.AbsolutPosition.X - (chunk.ChunkPosition.X * (CurrentWidth - 8)) + w < 0 || cell.AbsolutPosition.Y - (chunk.ChunkPosition.Y * CurrentHeight) + h < 0)
                                continue;

                            Console.SetCursorPosition(cell.AbsolutPosition.X - (chunk.ChunkPosition.X * (CurrentWidth - 8)) + w, cell.AbsolutPosition.Y - (chunk.ChunkPosition.Y * CurrentHeight) + h);

                            if (cell.Layer[w, h] != null)
                                Console.Write(BaseElementToChar(cell.Layer[w, h]));
                            else
                                Console.Write(BaseElementToChar(cell.Lines[w, h]));

                            cell.Invalid = false;
                        }
                    }
                }
            }

        }

        private void RecalculateChunks(Map map)
        {

            chunks?.Clear();

            var referenceCell = map?.Cells?.First();
            for (int w = 0; w < Math.Ceiling(map.Width * referenceCell.Width / (float)CurrentWidth); w++)
                for (int h = 0; h < Math.Ceiling(map.Height * referenceCell.Height / (float)CurrentHeight); h++)
                    chunks.Add(new Chunk(map.Cells, CurrentWidth - 8, CurrentHeight, new Position(w, h)));

            chunkChange = true;
        }

        private void DrawPlayerHealth(IBaseRole role)
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

        private void DrawPlayerInfo(Player player)
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

        private void DrawCellInventory(Inventory<MapCell> cellInventory)
        {
            var cellsToRefresh = cellInventory.Where(cell => cell.Invalid || chunkChange).ToArray();

            for (int i = 0; i < cellsToRefresh.Length; i++)
            {
                var cell = cellsToRefresh[i];
                if (cell is MapCell mapCell)
                    mapCell.NormalizeLayering();
                Console.ForegroundColor = Enum.TryParse(cell.Color.Name, out ConsoleColor color) ? color : ConsoleColor.White;
                for (int l = 0; l < cell.Width; l++)
                {
                    for (int h = 0; h < cell.Height; h++)
                    {
                        Console.SetCursorPosition(l + (i * 10), CurrentHeight + 4 + h);

                        if (cell.Layer[l, h] != null)
                            Console.Write(BaseElementToChar(cell.Layer[l, h]));
                        else
                            Console.Write(BaseElementToChar(cell.Lines[l, h]));

                        cell.Invalid = false;
                    }
                }
            }
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
                oldPlayerInventory = new BaseItemCell[itemInventory.Count];

            for (int i = 0; i < itemInventory.Count; i++)
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

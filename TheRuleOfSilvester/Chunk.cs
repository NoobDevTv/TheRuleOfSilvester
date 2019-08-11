using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester
{
    public class Chunk
    {
        public IEnumerable<Cell> Cells { get; private set; }
        public Position ChunkPosition { get; private set; }

        public Chunk(IEnumerable<Cell> cells, int chunkWidth, int chunkHeight, Position chunkPoint)
        {
            FillCells(cells, chunkWidth, chunkHeight, chunkPoint);
        }

        public void FillCells(IEnumerable<Cell> cells, int chunkWidth, int chunkHeight, Position chunkPoint)
        {
            ChunkPosition = chunkPoint;


            Cells = cells.Where(x =>
                x.AbsolutPosition.X / (chunkWidth) >= chunkPoint.X
                && x.AbsolutPosition.X / (chunkWidth) < chunkPoint.X + 1
                && x.AbsolutPosition.Y / chunkHeight >= chunkPoint.Y
                && x.AbsolutPosition.Y / chunkHeight < chunkPoint.Y + 1);
        }
    }
}
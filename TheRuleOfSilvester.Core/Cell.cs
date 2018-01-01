using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public abstract class Cell
    {
        //public const int HEIGHT = 3;
        //public const int WIDTH = 5;
        public Point Position { get; set; }
        public int Width => Lines.GetLength(1);
        public int Height => Lines.GetLength(0);
        public bool Invalid { get; set; }

        internal Map map;

        public string[,] Lines { get; internal set; }

        public Cell(int height, int width, Map map)
        {
            Lines = new string[height, width];
            Invalid = true;
            this.map = map;
        }
        public Cell(Map map) : this(3, 5, map)
        {
        }

        public void SetPosition(Point position)
        {
            Position = position;
            Invalid = true;
        }

        public virtual void Update(Game game)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public abstract class Cell : IDisposable
    {
        //public const int HEIGHT = 3;
        //public const int WIDTH = 5;
        public Point Position { get; set; }
        public int Width => Lines.GetLength(1);
        public int Height => Lines.GetLength(0);
        public bool Invalid { get; set; }
        public bool Movable { get; set; }
        public Color Color { get; set; }
        public Map Map { get; protected set; }

        public string[,] Lines { get; internal set; }

        protected bool disposed;

        public Cell(int height, int width, Map map)
        {
            Color = Color.White;
            Lines = new string[height, width];
            Invalid = true;
            Map = map;
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

        public override string ToString()
        {
            return $"{GetType()} | {Position.X} : {Position.Y}";
        }

        public virtual void Dispose()
        {
            if (disposed)
                return;

            Position = new Point(0);
            Invalid = false;
            Movable = false;
            Color = new Color();
            Map = null;
            Lines = null;

            disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}

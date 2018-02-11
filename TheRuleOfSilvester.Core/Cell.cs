using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public abstract class Cell : IDisposable, INotifyPropertyChanged
    {

        public Point Position { get; set; }
        public int Width => Lines.GetLength(0);
        public int Height => Lines.GetLength(1);
        public bool Invalid
        {
            get => invalid; set
            {
                if (invalid != value)
                {
                    invalid = value;
                    OnPropertyChanged("Invalid");
                }
            }
        }
        public bool Movable { get; set; }
        public Color Color { get; set; }
        public Map Map { get; protected set; }

        public char[,] Lines { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool disposed;

        private bool invalid;

        public Cell(int width, int height, Map map, bool movable = true)
        {
            Color = Color.White;
            Lines = new char[width, height];
            Invalid = true;
            Map = map;
            Movable = movable;
        }
        public Cell(Map map, bool movable = true) : this(5, 3, map, movable)
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

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

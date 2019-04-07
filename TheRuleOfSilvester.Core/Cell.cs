using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core
{
    public abstract class Cell : IDisposable, INotifyPropertyChanged
    {
        public Point Position { get => position; set => SetValue(value, ref position); }
        public Point AbsolutPosition => new Point(position.X * Width, position.Y * Height);
        public bool Invalid { get => invalid; set => SetValue(value, ref invalid); }


        public int Width => Lines.GetLength(0);
        public int Height => Lines.GetLength(1);
        public bool Movable { get; set; }
        public Color Color { get; set; }
        public Map Map { get; set; }

        public BaseElement[,] Lines { get; protected set; }

        public BaseElement[,] Layer { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangeEventHandler PropertyChange;

        protected bool disposed;

        private Point position;
        private bool invalid;

        public Cell(int width, int height, Map map, bool movable = true)
        {
            Color = Color.White;
            Lines = new BaseElement[width, height];
            Layer = new BaseElement[width, height];
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

        public override string ToString() => $"{GetType()} | {Position.X} : {Position.Y}";

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


        private void SetValue<T>(T value, ref T privateField, [CallerMemberName] string name = null)
        {
            if (value.Equals(privateField))
                return;

            PropertyChange?.Invoke(this, new PropertyChangeEventArgs(name, oldValue: privateField, newValue: value));

            privateField = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static bool IsOnPosition(Point pos, Cell x) =>
                 x.Position.X * x.Width <= pos.X && (x.Position.X * x.Width + x.Width) > pos.X
                 && x.Position.Y * x.Height <= pos.Y && (x.Position.Y * x.Height + x.Height) > pos.Y;
    }
}

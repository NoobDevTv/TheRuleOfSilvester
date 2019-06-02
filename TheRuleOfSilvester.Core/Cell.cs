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
            public Position Position { get => position; set => SetValue(value, ref position); }
            public Position AbsolutPosition => new Position(position.X * Width, position.Y * Height);
            public bool Invalid { get => invalid; set => SetValue(value, ref invalid); }


            public int Width { get; }
            public int Height { get; }
            public bool Movable { get; set; }
            public Color Color { get; set; }
            public Map Map { get; set; }

            public BaseElement[,] Lines { get; protected set; }

            public BaseElement[,] Layer { get; protected set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public event PropertyChangeEventHandler PropertyChange;

            protected bool disposed;

            private Position position;
            private bool invalid;

            public Cell(int width, int height, Map map, bool movable = true)
            {
                Color = Color.White;
                Lines = new BaseElement[width, height];
                Width = width;
                Height = height;
                Layer = new BaseElement[width, height];
                Invalid = true;
                Map = map;
                Movable = movable;
            }
            public Cell(Map map, bool movable = true) : this(5, 3, map, movable)
            {

            }

            public void SetPosition(Position position)
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

                Position = new Position(0);
                Invalid = false;
                Movable = false;
                Color = new Color();
                Map = null;

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

            public static bool IsOnPosition(Position pos, Cell x) =>
                     x.Position.X * x.Width <= pos.X && (x.Position.X * x.Width + x.Width) > pos.X
                     && x.Position.Y * x.Height <= pos.Y && (x.Position.Y * x.Height + x.Height) > pos.Y;
        }
    }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Demo.Views
{
    internal class GridView : View
    {
        private readonly List<Item> items;

        public GridView(SizeUnit sizeUnit, params Style[] styles) : base()
        {
            items = new();
        }

        public void Add(View view, int column = 0, int row = 0)
            => items.Add(new Item(view, column, row));

        public void Remove(View view)
            => items.Remove(view);

        public override IObservable<GraphicViewState> Show()
            => Observable.Merge(items.Select(i => i.View.Show()));

        public record Style(StyleType Type, int Order, int Size = 1);

        public enum StyleType { Row, Column }

        public enum SizeUnit { Auto, Relative, Absolute }

        private readonly struct Item : IEquatable<Item>
        {
            public readonly View View { get; }
            public readonly int Column { get; }
            public readonly int Row { get; }

            public Item(View view, int column, int row)
            {
                View = view;
                Column = column;
                Row = row;
            }

            public override bool Equals(object obj)
                => obj is Item item && Equals(item);
            public bool Equals(Item other)
                => EqualityComparer<View>.Default.Equals(View, other.View);
            public override int GetHashCode()
                => HashCode.Combine(View);

            public static bool operator ==(Item left, Item right)
                => left.Equals(right);
            public static bool operator !=(Item left, Item right)
                => !(left == right);

            public static implicit operator View(Item item)
                => item.View;

            public static implicit operator Item(View view)
                => new Item(view, 0, 0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlazingOrchard.DisplayManagement.Zones;

namespace BlazingOrchard.DisplayManagement.Shapes
{
    public class Shape : Composite, IShape, IPositioned, IEnumerable<object>
    {
        public ShapeMetadata Metadata { get; set; } = new ShapeMetadata();
        public bool HasItems => _items.Count > 0;
        private List<IPositioned> _items = new List<IPositioned>();
        private bool _sorted;

        public string? Position
        {
            get => Metadata.Position;
            set => Metadata.Position = value;
        }

        public IEnumerable<dynamic> Items
        {
            get
            {
                if (!_sorted)
                {
                    _items = _items.OrderBy(x => x, FlatPositionComparer.Instance).ToList();
                    _sorted = true;
                }

                return _items;
            }
        }

        public virtual Shape Add(object? item, string? position = default)
        {
            if (item == null)
                return this;

            if (position == null)
                position = "";

            _sorted = false;

            if (item is string s)
            {
                _items.Add(new PositionWrapper(s, position));
            }
            else
            {
                if (item is IPositioned shape)
                {
                    shape.Position = position;
                    _items.Add(shape);
                }
            }

            return this;
        }

        public Shape AddRange(IEnumerable<object> items, string? position = default)
        {
            foreach (var item in items)
                Add(item, position);

            return this;
        }

        public void Remove(string shapeName)
        {
            for (var i = _items.Count - 1; i >= 0; i--)
            {
                if (_items[i] is Shape shape && shape.Metadata.Name == shapeName)
                {
                    _items.RemoveAt(i);
                    return;
                }
            }
        }

        public IShape? Named(string shapeName)
        {
            foreach (var t in _items)
                if (t is IShape shape && shape.Metadata.Name == shapeName)
                    return shape;

            return null;
        }

        public IShape? NormalizedNamed(string shapeName)
        {
            foreach (var t in _items)
                if (t is IShape shape && shape.Metadata.Name?.Replace("__", "-") == shapeName)
                    return shape;

            return null;
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            if (!_sorted)
            {
                _items = _items.OrderBy(x => x, FlatPositionComparer.Instance).ToList();
                _sorted = true;
            }

            return _items.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            if (!_sorted)
            {
                _items = _items.OrderBy(x => x, FlatPositionComparer.Instance).ToList();
                _sorted = true;
            }

            return _items.GetEnumerator();
        }
    }
}
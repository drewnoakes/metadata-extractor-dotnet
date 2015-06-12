using System;
using System.Collections.Generic;

namespace Sharpen
{
    public class EnumeratorWrapper<T> : Iterator<T>
    {
        readonly object _collection;
        IEnumerator<T> _e;
        T _lastVal;
        bool _more;
        bool _copied;

        public EnumeratorWrapper (object collection, IEnumerator<T> e)
        {
            _e = e;
            _collection = collection;
            _more = e.MoveNext();
        }

        public override bool HasNext ()
        {
            return _more;
        }

        public override T Next ()
        {
            if (!_more)
                throw new InvalidOperationException();
            _lastVal = _e.Current;
            _more = _e.MoveNext ();
            return _lastVal;
        }

        public override void Remove ()
        {
            var col = _collection as ICollection<T>;
            if (col == null) {
                throw new NotSupportedException ();
            }
            if (_more && !_copied) {
                // Read the remaining elements, since the current enumerator
                // will be invalid after removing the element
                var remaining = new List<T> ();
                do {
                    remaining.Add (_e.Current);
                } while (_e.MoveNext ());
                _e = remaining.GetEnumerator ();
                _e.MoveNext ();
                _copied = true;
            }
            col.Remove (_lastVal);
        }
    }
}

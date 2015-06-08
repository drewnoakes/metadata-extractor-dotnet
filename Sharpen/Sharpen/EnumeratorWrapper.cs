using System;
using System.Collections;
using System.Collections.Generic;

namespace Sharpen
{
    public class EnumeratorWrapper<T> : Iterator<T>
    {
        readonly object collection;
        IEnumerator<T> e;
        T lastVal;
        bool more;
        bool copied;

        public EnumeratorWrapper (object collection, IEnumerator<T> e)
        {
            this.e = e;
            this.collection = collection;
            this.more = e.MoveNext();
        }

        public override bool HasNext ()
        {
            return this.more;
        }

        public override T Next ()
        {
            if (!more)
                throw new NoSuchElementException ();
            lastVal = e.Current;
            more = e.MoveNext ();
            return lastVal;
        }

        public override void Remove ()
        {
            ICollection<T> col = this.collection as ICollection<T>;
            if (col == null) {
                throw new NotSupportedException ();
            }
            if (more && !copied) {
                // Read the remaining elements, since the current enumerator
                // will be invalid after removing the element
                List<T> remaining = new List<T> ();
                do {
                    remaining.Add (e.Current);
                } while (e.MoveNext ());
                e = remaining.GetEnumerator ();
                e.MoveNext ();
                copied = true;
            }
            col.Remove (lastVal);
        }
    }

    public class EnumeratorWrapper : Iterator
    {
        readonly object collection;
        IEnumerator e;
        object lastVal;
        bool more;
        bool copied;
        private object lastValue;

        public EnumeratorWrapper (object collection, IEnumerator e)
        {
            this.e = e;
            this.collection = collection;
            this.more = e.MoveNext();
        }

        public bool HasNext ()
        {
            return this.more;
        }

        public object Next ()
        {
            if (!more)
                throw new NoSuchElementException ();
            lastVal = e.Current;
            more = e.MoveNext ();
            return lastVal;
        }

        public void Remove ()
        {
            ICollection col = this.collection as ICollection;
            if (col == null) {
                throw new NotSupportedException ();
            }
            if (more && !copied) {
                // Read the remaining elements, since the current enumerator
                // will be invalid after removing the element
                IList remaining = new ArrayList ();
                do {
                    remaining.Add (e.Current);
                } while (e.MoveNext ());
                e = remaining.GetEnumerator ();
                e.MoveNext ();
                copied = true;
            }

            if (col is IDictionary)
            {
                ((IDictionary)col).Remove(lastVal);
                return;
            }

            if (col is IList)
            {
                ((IList)col).Remove(lastVal);
                return;
            }

            throw new NotSupportedException();
        }
    }
}

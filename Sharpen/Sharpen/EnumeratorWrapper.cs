using System.Collections;

namespace Sharpen
{
	using System;
	using System.Collections.Generic;

	public class EnumeratorWrapper<T> : Iterator<T>
	{
		object collection;
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

	    public EnumeratorWrapper(IList<T> list) : this(list, list.GetEnumerator())
	    {
	    }

	    public EnumeratorWrapper(IEnumerable<T> keys) : this(keys, GetEnumerator(keys))
	    {
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

        private static IEnumerator<T> GetEnumerator<T>(IEnumerable<T> keys)
        {
            return keys.Iterator();
        }
	}
    
    public class EnumeratorWrapper : Iterator
	{
		object collection;
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

	    public EnumeratorWrapper(IList list) : this(list, list.GetEnumerator())
	    {
	    }

	    public EnumeratorWrapper(IEnumerable keys) : this(keys, GetEnumerator(keys))
	    {
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

        private static IEnumerator GetEnumerator(IEnumerable keys)
        {
            return keys.GetEnumerator();
        }

        public bool MoveNext()
        {
            if (HasNext())
            {
                lastValue = Next();
                return true;
            }
            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public object Current
        {
            get { return lastValue; }
        }
	}
}

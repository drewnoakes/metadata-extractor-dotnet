using System.Collections;
using System.Collections.Generic;

namespace Sharpen
{
    public class ListIterator : Iterator<object>
    {
        private readonly IList list;
        private int pos;

        public ListIterator(IList list, int n)
        {
            this.list = list;
            this.pos = n;
        }

        public ListIterator(IList list)
            : this(list, -1)
        {
        }

        public bool HasPrevious()
        {
            return (this.pos > 0);
        }

        public object Previous()
        {
            pos--;
            return list[pos];
        }

        public override object Next()
        {
            pos++;
            return list[pos];
        }

        public override void Remove()
        {
            list.RemoveAt(pos);
        }

        public override bool HasNext()
        {
            return (this.pos < list.Count - 1);
        }

        public void Set(object val)
        {
            list[pos] = val;
        }
    }

    public class ListIterator<T>
    {
        private readonly IList<T> list;
        private int pos;

        public ListIterator (IList<T> list, int n)
        {
            this.list = list;
            this.pos = n;
        }

        public ListIterator (IList<T> list) : this(list, -1)
        {
        }

        public bool HasPrevious ()
        {
            return (this.pos > 0);
        }

        public T Previous ()
        {
            pos--;
            return list[pos];
        }

        public T Next()
        {
            pos++;
            return list[pos];
        }

        public void Set(T val)
        {
            list[pos] = val;
        }
    }
}

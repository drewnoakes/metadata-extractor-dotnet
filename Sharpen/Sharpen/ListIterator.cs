using System.Collections;

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
}

using System.Collections.Generic;

namespace Sharpen
{
    public class EnumerableWrapper<T> : Iterable<T>
    {
        private IEnumerable<T> e;

        public EnumerableWrapper (IEnumerable<T> e)
        {
            this.e = e;
        }

        public override Iterator<T> Iterator ()
        {
            return new EnumeratorWrapper<T> (this.e, this.e.GetEnumerator ());
        }
    }
}

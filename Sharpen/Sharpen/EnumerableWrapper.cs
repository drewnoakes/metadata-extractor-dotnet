using System.Collections.Generic;

namespace Sharpen
{
    public class EnumerableWrapper<T> : Iterable<T>
    {
        private readonly IEnumerable<T> _e;

        public EnumerableWrapper (IEnumerable<T> e)
        {
            this._e = e;
        }

        public override Iterator<T> Iterator ()
        {
            return new EnumeratorWrapper<T> (this._e, this._e.GetEnumerator ());
        }
    }
}

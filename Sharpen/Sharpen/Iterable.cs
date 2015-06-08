using System.Collections;
using System.Collections.Generic;

namespace Sharpen
{
    public abstract class Iterable<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator ()
        {
            return Iterator ();
        }

        public abstract Iterator<T> Iterator ();

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return Iterator ();
        }
    }
}

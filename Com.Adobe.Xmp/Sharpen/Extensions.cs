using System.Collections.Generic;

namespace Sharpen
{
    public static class Extensions
    {
        public static Iterator<T> Iterator<T>(this IEnumerable<T> col)
        {
            return new EnumeratorWrapper<T>(col, col.GetEnumerator());
        }
    }
}
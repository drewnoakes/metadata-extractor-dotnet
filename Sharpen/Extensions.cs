using System.Collections.Generic;
using JetBrains.Annotations;

namespace Sharpen
{
    public static class Extensions
    {
        [CanBeNull]
        public static TU GetOrNull<T, TU>(this IDictionary<T, TU> d, T key) where TU : class
        {
            TU val;
            return d.TryGetValue(key, out val) ? val : null;
        }

        public static Iterator<T> Iterator<T>(this IEnumerable<T> col)
        {
            return new EnumeratorWrapper<T>(col, col.GetEnumerator());
        }
    }
}
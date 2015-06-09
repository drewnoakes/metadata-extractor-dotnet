using System;
using System.Linq;

namespace Sharpen
{
    public static class Arrays
    {
        public static bool Equals<T> (T[] a1, T[] a2)
        {
            return a1.SequenceEqual(a2);
        }

        public static void Sort (string[] array)
        {
            Array.Sort (array, string.CompareOrdinal);
        }

        public static void Sort<T> (T[] array)
        {
            Array.Sort (array);
        }

        public static void Sort<T> (T[] array, int start, int count)
        {
            Array.Sort (array, start, count);
        }

        /// <summary>
        /// Counts array hash code
        /// </summary>
        /// <remarks>Implementation ported from openjdk source</remarks>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int HashCode(sbyte[] a)
        {
            if (a == null)
                return 0;

            return a.Aggregate(1, (current, element) => 31*current + element);
        }
    }
}

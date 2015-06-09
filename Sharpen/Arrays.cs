using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpen
{
    public static class Arrays
    {
        public static List<T> AsList<T> (params T[] array)
        {
            return array.ToList ();
        }

        public static bool Equals<T> (T[] a1, T[] a2)
        {
            if (a1.Length != a2.Length) {
                return false;
            }
            return !a1.Where((t, i) => !t.Equals(a2[i])).Any();
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

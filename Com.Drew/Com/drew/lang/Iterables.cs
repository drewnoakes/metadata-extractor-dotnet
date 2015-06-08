using System.Collections.Generic;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class Iterables
    {
        public static IList<E> ToList<E>(Iterable<E> iterable)
        {
            AList<E> list = new AList<E>();
            foreach (E item in iterable)
            {
                list.Add(item);
            }
            return list;
        }

        public static ICollection<E> ToSet<E>(Iterable<E> iterable)
        {
            HashSet<E> set = new HashSet<E>();
            foreach (E item in iterable)
            {
                set.Add(item);
            }
            return set;
        }
    }
}

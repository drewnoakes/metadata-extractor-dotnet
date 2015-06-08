using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sharpen
{
    public static class Collections
    {
        static readonly IList empty = new object[0];

        public static IList EMPTY_SET
        {
            get { return empty; }
        }

        public static bool AddAll<T> (ICollection<T> list, IEnumerable toAdd)
        {
            foreach (T t in toAdd)
                list.Add (t);
            return true;
        }

//        public static V Remove<K, V> (IDictionary<K, V> map, K toRemove) where K : class
        public static V Remove<K, V> (IDictionary<K, V> map, K toRemove)
        {
            V local;
            if (map.TryGetValue (toRemove, out local)) {
                map.Remove (toRemove);
                return local;
            }
            return default(V);
        }

        public static object Remove(IDictionary map, object toRemove)
        {
            object local = map[toRemove];
            map.Remove (toRemove);
            //return default(V);
            return local;
        }

        public static object[] ToArray (IList list, object[] result)
        {
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = list[i];
            }

            return result;
        }

        public static U[] ToArray<T,U> (ICollection<T> list, U[] res) where T:U
        {
            if (res.Length < list.Count)
                res = new U [list.Count];

            int n = 0;
            foreach (T t in list)
                res [n++] = t;

            if (res.Length > list.Count)
                res [list.Count] = default (T);
            return res;
        }

        public static IList EmptyList()
        {
            return EMPTY_SET;
        }

        public static ICollection<T> UnmodifiableCollection<T> (ICollection<T> list)
        {
            return list;
        }

        public static IList UnmodifiableList (IList list)
        {
            return new ReadOnlyCollection<object>(ConvertToGenericList(list));
        }

        public static ICollection<T> UnmodifiableSet<T> (ICollection<T> list)
        {
            return list;
        }

        public static IDictionary UnmodifiableMap(IDictionary dict)
        {
            return dict;
        }

        public static IDictionary<K,V> UnmodifiableMap<K,V> (IDictionary<K,V> dict)
        {
            return dict;
        }

        private static IList<object> ConvertToGenericList(IList list)
        {
            List<object> result = new List<object>();

            foreach (object item in list)
            {
                result.Add(item);
            }

            return result;
        }
    }
}

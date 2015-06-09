using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace Sharpen
{
    public static class Runtime
    {
        public static byte[] GetBytesForString (string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static void PrintStackTrace (Exception ex)
        {
            Console.WriteLine (ex);
        }

        public static void PrintStackTrace (Exception ex, TextWriter tw)
        {
            tw.WriteLine (ex);
        }

        public static string Substring (string str, int index)
        {
            return str.Substring (index);
        }

        public static string Substring(string str, int index, int endIndex)
        {
            return str.Substring (index, endIndex - index);
        }

        public static void SetCharAt (StringBuilder sb, int index, char c)
        {
            sb [index] = c;
        }

        public static bool EqualsIgnoreCase (string s1, string s2)
        {
            return s1.Equals (s2, StringComparison.CurrentCultureIgnoreCase);
        }

        public static int GetArrayLength(object array)
        {
            if (array.GetType().IsArray)
            {
                var collection = array as ICollection;
                if (collection != null)
                {
                    return collection.Count;
                }
            }

            throw new NotSupportedException();
        }

        private static T GetArrayValue<T>(object array, int index)
        {
            if (array.GetType().IsArray)
            {
                var collection = array as ICollection;
                if (collection != null)
                {
                    return collection.Cast<T>().ElementAt(index);
                }

                var enumerable = array as IEnumerable;
                if (enumerable != null)
                {
                    return enumerable.Cast<T>().ElementAt(index);
                }
            }

            throw new NotSupportedException();
        }

        public static object GetArrayValue(object array, int index)
        {
            return GetArrayValue<object>(array, index);
        }

        public static int GetInt(object array, int index)
        {
            return GetArrayValue<int>(array, index);
        }

        public static short GetShort(object array, int index)
        {
            return GetArrayValue<short>(array, index);
        }

        public static long GetLong(object array, int index)
        {
            return GetArrayValue<long>(array, index);
        }

        public static float GetFloat(object array, int index)
        {
            return GetArrayValue<float>(array, index);
        }

        public static double GetDouble(object array, int index)
        {
            return GetArrayValue<double>(array, index);
        }

        public static byte GetByte(object array, int index)
        {
            return GetArrayValue<byte>(array, index);
        }
    }
}

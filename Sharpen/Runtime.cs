using System;
using System.Collections;
using System.Linq;

namespace Sharpen
{
    public static class Runtime
    {
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

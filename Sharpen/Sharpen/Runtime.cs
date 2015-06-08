using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace Sharpen
{
    public static class Runtime
    {
        static Hashtable _properties;

        public static Hashtable GetProperties ()
        {
            if (_properties == null) {
                _properties = new Hashtable ();
                _properties ["jgit.fs.debug"] = "false";
                var home = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile).Trim ();
                if (string.IsNullOrEmpty (home))
                    home = Environment.GetFolderPath (Environment.SpecialFolder.Personal).Trim ();
                _properties ["user.home"] = home;
                _properties ["java.library.path"] = Environment.GetEnvironmentVariable ("PATH");
                if (Path.DirectorySeparatorChar != '\\')
                    _properties ["os.name"] = "Unix";
                else
                    _properties ["os.name"] = "Windows";
                _properties["file.encoding"] = Encoding.UTF8.BodyName;
            }
            return _properties;
        }

        public static string GetProperty (string key)
        {
            return ((string) GetProperties()[key]);
        }

        public static sbyte[] GetBytesForString (string str)
        {
            return Extensions.ConvertToByteArray(Encoding.UTF8.GetBytes (str));
        }

        public static sbyte[] GetBytesForString (string str, string encoding)
        {
            return Extensions.ConvertToByteArray(Encoding.GetEncoding(encoding).GetBytes(str));
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

        public static long NanoTime ()
        {
            return Environment.TickCount * 1000 * 1000;
        }

        public static string GetStringForBytes(sbyte[] sbytes, int start, int len)
        {
            return GetStringForBytes(Extensions.ConvertToByteArray(sbytes), start, len);
        }

        public static string GetStringForBytes(sbyte[] sbytes, string encoding)
        {
            return GetStringForBytes(Extensions.ConvertToByteArray(sbytes), encoding);
        }

        public static string GetStringForBytes(sbyte[] sbytes, int start, int len, string encoding)
        {
            return GetStringForBytes(Extensions.ConvertToByteArray(sbytes), start, len, encoding);
        }

        public static string GetStringForBytes (sbyte[] chars)
        {
            return Encoding.UTF8.GetString(Extensions.ConvertToByteArray(chars));
        }

        public static string GetStringForBytes (byte[] chars, string encoding)
        {
            return GetEncoding (encoding).GetString (chars);
        }

        public static string GetStringForBytes (byte[] chars, int start, int len)
        {
            return Encoding.UTF8.GetString (chars, start, len);
        }

        public static string GetStringForBytes (byte[] chars, int start, int len, string encoding)
        {
            return GetEncoding (encoding).Decode (chars, start, len);
        }

        public static Encoding GetEncoding (string name)
        {
            try
            {
//            Encoding e = Encoding.GetEncoding (name, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                Encoding e = Encoding.GetEncoding(name.Replace('_', '-'));
                if (e is UTF8Encoding)
                    return new UTF8Encoding(false, true);
                return e;
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName.Equals("name"))
                {
                    throw new UnsupportedEncodingException();
                }

                throw;
            }
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

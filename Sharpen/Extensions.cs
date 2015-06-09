using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using JetBrains.Annotations;

namespace Sharpen
{
    public static class Extensions
    {
        private static readonly long EpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        private static string Decode(this Encoding e, byte[] chars, int start, int len)
        {
            try
            {
                byte[] bom = e.GetPreamble();
                if (bom != null && bom.Length > 0)
                {
                    if (len >= bom.Length)
                    {
                        int pos = start;
                        bool hasBom = true;
                        for (int n = 0; n < bom.Length && hasBom; n++)
                        {
                            if (bom[n] != chars[pos++])
                                hasBom = false;
                        }
                        if (hasBom)
                        {
                            len -= pos - start;
                            start = pos;
                        }
                    }
                }
                return e.GetString(chars, start, len);
            }
            catch (DecoderFallbackException dfe)
            {
                throw new CharacterCodingException();
            }
        }

        [CanBeNull]
        public static TU GetOrNull<T, TU>(this IDictionary<T, TU> d, T key) where TU : class
        {
            TU val;
            return d.TryGetValue(key, out val) ? val : null;
        }

        public static CultureInfo GetEnglishCulture()
        {
            return CultureInfo.GetCultureInfo("en-US");
        }

        public static TimeZoneInfo GetTimeZone(string tzone)
        {
            if (tzone.Equals("GMT"))
            {
                tzone = "GMT Standard Time";
            }

            try
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(tzone);
                return tz;
            }
            catch
            {
                // Not found
            }

            // Mono and Java allow you to specify timezones by short id (i.e. EST instead of Eastern Standard Time).
            // Mono on Windows and the microsoft framewokr on windows do not allow this. This hack is to compensate
            // for that and allow you to match 'EST' to 'Eastern Standard Time' by matching the first letter of each
            // word to the corresponding character in the short string. Bleh.
            if (tzone.Length <= 4)
            {
                foreach (var timezone in TimeZoneInfo.GetSystemTimeZones())
                {
                    var parts = timezone.Id.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == tzone.Length)
                    {
                        bool found = true;
                        for (int i = 0; i < parts.Length; i++)
                            found &= parts[i][0] == tzone[i];

                        if (found)
                            return timezone;
                    }
                }
            }
            char[] separator = new char[] {':'};
            string[] strArray = tzone.Substring(4).Split(separator);
            int hours, minutes;
            if (strArray.Length == 1 && strArray[0].Length > 2)
            {
                hours = int.Parse(strArray[0].Substring(0, 2));
                minutes = int.Parse(strArray[0].Substring(2));
            }
            else
            {
                hours = int.Parse(strArray[0]);
                minutes = (strArray.Length <= 1) ? 0 : int.Parse(strArray[1]);
            }
            TimeSpan t = new TimeSpan(0, hours, minutes, 0, 0);
            if (tzone[3] == '-')
                t = -t;
            return TimeZoneInfo.CreateCustomTimeZone(tzone, t, tzone, tzone);
        }

        public static IIterator Iterator(this ICollection col)
        {
            return new EnumeratorWrapper(col, col.GetEnumerator());
        }

        public static Iterator<T> Iterator<T>(this ICollection<T> col)
        {
            return new EnumeratorWrapper<T>(col, col.GetEnumerator());
        }

        public static ListIterator ListIterator(this IList col)
        {
            return new ListIterator(col);
        }

        public static DateTime CreateDate(long milliSecondsSinceEpoch)
        {
            long num = EpochTicks + (milliSecondsSinceEpoch*10000);
            return new DateTime(num);
        }

        public static DateTimeOffset MillisToDateTimeOffset(long milliSecondsSinceEpoch, long offsetMinutes)
        {
            TimeSpan offset = TimeSpan.FromMinutes((double) offsetMinutes);
            long num = EpochTicks + (milliSecondsSinceEpoch*10000);
            return new DateTimeOffset(num + offset.Ticks, offset);
        }

        public static void Sort(this IList list)
        {
            IList sorted = new ArrayList(list);
            sorted.Sort();
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = sorted[i];
            }
        }

        public static string[] Split(this string str, string regex, int limit)
        {
            Regex rgx = new Regex(regex);
            List<string> list = new List<string>();
            int startIndex = 0;
            if (limit != 1)
            {
                int nm = 1;
                foreach (Match match in rgx.Matches(str))
                {
                    list.Add(str.Substring(startIndex, match.Index - startIndex));
                    startIndex = match.Index + match.Length;
                    if (limit > 0 && ++nm == limit)
                        break;
                }
            }
            if (startIndex < str.Length)
            {
                list.Add(str.Substring(startIndex));
            }
            if (limit >= 0)
            {
                int count = list.Count - 1;
                while ((count >= 0) && (list[count].Length == 0))
                {
                    count--;
                }
                list.RemoveRange(count + 1, (list.Count - count) - 1);
            }
            return list.ToArray();
        }

        public static string ConvertToString(DateTime val)
        {
            //  EEE MMM dd HH:mm:ss zzz yyyy
            return val.ToString("ddd MMM dd HH:mm:ss zzz yyyy");
        }

        /// <summary>
        /// Returns all public static fields values with specified type
        /// </summary>
        /// <typeparam name="T">values type</typeparam>
        /// <param name="type">values source</param>
        /// <returns></returns>
        public static T[] GetEnumConstants<T>(this Type type)
        {
            var result = new List<T>();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof (T))
                {
                    result.Add((T) field.GetValue(null));
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns all public static fields values with specified type
        /// </summary>
        /// <typeparam name="T">values type</typeparam>
        /// <param name="name">Constant name</param>
        /// <returns></returns>
        public static T GetEnumConstantByName<T>(string name)
        {
            var type = typeof (T);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof (T) && field.Name.Equals(name))
                {
                    return (T) field.GetValue(null);
                }
            }

            return default(T);
        }

        public static int Digit(char ch, int radix)
        {
            //  http://stackoverflow.com/questions/1021645/is-there-something-like-javas-character-digitchar-ch-int-radix-in-c
            return Convert.ToInt32(ch.ToString(), radix);
        }

        public static decimal Compare(double d1, double d2)
        {
            //  http://stackoverflow.com/questions/1398753/comparing-double-values-in-c-sharp

            double diff = d1 - d2;

            if (Math.Abs(diff) < 0.0001)
            {
                return 0;
            }

            return Math.Sign(diff);
        }

        public static bool IsNumber(this object value)
        {
            try
            {
                var number = Number.GetInstance(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAttributes(this XmlNode node)
        {
            return node.Attributes != null && node.Attributes.Count > 0;
        }

        internal static void CopyCastBuffer(byte[] buffer, int offset, int len, byte[] targetBuffer, int targetOffset)
        {
            if (offset < 0 || len < 0 || offset + len > buffer.Length || targetOffset < 0 || targetOffset + len > targetBuffer.Length)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < len; i++)
            {
                targetBuffer[i + targetOffset] = (byte)buffer[offset + i];
            }
        }
    }
}
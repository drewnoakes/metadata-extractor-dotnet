using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Sharpen
{
    public static class Extensions
    {
        private static readonly long EpochTicks;

        //  The format specifiers which do not correspond to arguments have the following syntax:
        //  %[flags][width]conversion
        //  http://docs.oracle.com/javase/7/docs/api/java/util/Formatter.html
        private const string FlagRegexPattern = "[-\\#+\\s0,]*";
        private const string WidthRegexPattern = "\\d*\\.*\\d*";
        private const string ConversionRegexPattern = "(?i:[sdnfx]{1})";
        private static readonly Regex StringSplitter = new Regex("(%" + FlagRegexPattern + WidthRegexPattern + ConversionRegexPattern + ")", RegexOptions.Compiled);
        private static readonly Regex FormatSplitter = new Regex("("+FlagRegexPattern+")("+WidthRegexPattern+")("+ConversionRegexPattern+")", RegexOptions.Compiled);

        static Extensions()
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            EpochTicks = time.Ticks;
        }

        public static void Add(this IList list, int index, object item)
        {
            list.Insert(index, item);
        }

        public static CultureInfo CreateLocale(string language, string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                return CultureInfo.GetCultureInfoByIetfLanguageTag(language);
            }

            return CultureInfo.GetCultureInfo(string.Format("{0}-{1}", language, country));
        }

        public static string Decode(this Encoding e, byte[] chars, int start, int len)
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

        public static string Decode(this Encoding e, ByteBuffer buffer)
        {
            return e.Decode(buffer.Array(), buffer.ArrayOffset() + buffer.Position(), buffer.Limit() - buffer.Position());
        }

        private static readonly UTF8Encoding Utf8Encoder = new UTF8Encoding(false, true);

        public static Encoding GetEncoding(string name)
        {
//            Encoding e = Encoding.GetEncoding (name, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            try
            {
                Encoding e = Encoding.GetEncoding(name.Replace('_', '-'));
                if (e is UTF8Encoding)
                    return Utf8Encoder;
                return e;
            }
            catch (ArgumentException)
            {
                throw new UnsupportedCharsetException(name);
            }
        }

        public static ICollection<KeyValuePair<T, TU>> EntrySet<T, TU>(this IDictionary<T, TU> s)
        {
            return s;
        }

        public static bool ContainsKey(this IDictionary d, object key)
        {
            return d.Contains(key);
        }

        public static TU Get<T, TU>(this IDictionary<T, TU> d, T key)
        {
            TU val;
            d.TryGetValue(key, out val);
            return val;
        }

        public static object Get(this IDictionary d, object key)
        {
            return d[key];
        }

        public static TU Put<T, TU>(this IDictionary<T, TU> d, T key, TU value)
        {
            TU old;
            d.TryGetValue(key, out old);
            d[key] = value;
            return old;
        }

        public static object Put(this IDictionary d, object key, object value)
        {
            object old = d[key];
            d[key] = value;
            return old;
        }

        public static CultureInfo GetEnglishCulture()
        {
            return CultureInfo.GetCultureInfo("en-US");
        }

        public static int GetOffset(this TimeZoneInfo tzone, long date)
        {
            return (int) tzone.GetUtcOffset(MillisToDateTimeOffset(date, 0).DateTime).TotalMilliseconds;
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

        public static bool IsEmpty(this ICollection col)
        {
            return col.Count == 0;
        }

        public static bool IsEmpty<T>(this ICollection<T> col)
        {
            return col.Count == 0;
        }

        public static bool IsEmpty<T>(this Stack<T> col)
        {
            return col.Count == 0;
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

        public static CharsetDecoder NewDecoder(this Encoding enc)
        {
            return new CharsetDecoder(enc);
        }

        public static T Remove<T>(this IList<T> list, int i)
        {
            T old;
            try
            {
                old = list[i];
                list.RemoveAt(i);
            }
            catch (IndexOutOfRangeException)
            {
                throw new NoSuchElementException();
            }
            return old;
        }

        public static object Set(this IList list, int index, object item)
        {
            object old = list[index];
            list[index] = item;
            return old;
        }

        // Conflicts with System.Linq.Enumerable.Contains<T>(System.Collections.Generic.IEnumerable<T>, T)
        /* public static bool Contains<T>(this ICollection<T> col, object item)
        {
            if (!(item is T))
                return false;
            return col.Any(n => (object.ReferenceEquals(n, item)) || n.Equals(item));
        }*/

        public static void Sort(this IList list)
        {
            IList sorted = new ArrayList(list);
            sorted.Sort();
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = sorted[i];
            }
        }

        public static void Sort<T>(this IList<T> list)
        {
            List<T> sorted = new List<T>(list);
            sorted.Sort();
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = sorted[i];
            }
        }

        public static void Sort<T>(this IList<T> list, IComparer<T> comparer)
        {
            List<T> sorted = new List<T>(list);
            sorted.Sort(comparer);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = sorted[i];
            }
        }

        public static string[] Split(this string str, string regex)
        {
            return str.Split(regex, 0);
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

        public static CharSequence[] ToCharSequence(this IEnumerable<string> strArr)
        {
            return (from str in strArr select (CharSequence) str).ToArray();
        }

        public static long ToMillisecondsSinceEpoch(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("dateTime is expected to be expressed as a UTC DateTime", "dateTime");
            }
            return
                new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeSpan.Zero)
                    .ToMillisecondsSinceEpoch();
        }

        public static long ToMillisecondsSinceEpoch(this DateTimeOffset dateTimeOffset)
        {
            return (((dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks) - EpochTicks)/TimeSpan.TicksPerMillisecond);
        }

        public static string ToHexString(int val)
        {
            return Convert.ToString(val, 16);
        }

        public static string ConvertToString(int val)
        {
            return val.ToString();
        }

        public static string ConvertToString(int? val)
        {
            return ConvertToString(val.Value);
        }

        public static string ConvertToString(float val)
        {
            return val.ToString("0.0###########");
        }

        public static string ConvertToString(DateTime? val)
        {
            return ConvertToString(val.Value);
        }

        public static string ConvertToString(DateTime val)
        {
            //  EEE MMM dd HH:mm:ss zzz yyyy
            return val.ToString("ddd MMM dd HH:mm:ss zzz yyyy");
        }

        public static string ConvertToString(float? val)
        {
            return ConvertToString(val.Value);
        }

        public static string ConvertToString(object val)
        {
            return val.ToString();
        }

        public static T ValueOf<T>(T val)
        {
            return val;
        }

        public static int ValueOf(string val)
        {
            return Convert.ToInt32(val);
        }

        public static string GetImplementationVersion(this Assembly asm)
        {
            return asm.GetName().Version.ToString();
        }

        public static HttpUrlConnection OpenConnection(this Uri uri)
        {
            return new HttpsUrlConnection(uri);
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

        public static int CompareTo(this int? value, int? compareVal)
        {
            return value.Value.CompareTo(compareVal.Value);
        }

        public static long DoubleToLongBits(double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        public static double LongBitsToDouble(long value)
        {
            return BitConverter.Int64BitsToDouble(value);
        }

        public static float IntBitsToFloat(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.ToSingle(bytes, 0);
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

        /// <summary>
        /// Call this trim method instead of standard .Net string.Trim(),
        /// becase .Net string.Trim() method removes only spaces and the Java String.Trim()
        /// removes all chars less than space ' '
        /// </summary>
        /// <remarks>Implementation ported from openjdk source</remarks>
        /// <param name="str">Source string</param>
        /// <returns>Trimmed string</returns>
        public static string Trim(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            int len = str.Length;
            int st = 0;

            while ((st < len) && (str[st] <= ' '))
            {
                st++;
            }

            while ((st < len) && (str[len - 1] <= ' '))
            {
                len--;
            }

            return ((st > 0) || (len < str.Length)) ? str.Substring(st, len - st) : str;
        }

        public static byte[] ConvertToByteArray(sbyte[] sbytes)
        {
            return Array.ConvertAll(sbytes, sb => (byte)sb);
        }

        public static sbyte[] ConvertToByteArray(byte[] bytes)
        {
            return Array.ConvertAll(bytes, b => (sbyte)b);
        }

        public static void Copy(byte[] buffer, sbyte[] sbuffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                sbuffer[i] = (sbyte) buffer[i];
            }
        }

        internal static void CopyCastBuffer(byte[] buffer, int offset, int len, sbyte[] targetBuffer, int targetOffset)
        {
            if (offset < 0 || len < 0 || offset + len > buffer.Length || targetOffset < 0 || targetOffset + len > targetBuffer.Length) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < len; i++)
            {
                targetBuffer[i + targetOffset] = (sbyte)buffer[offset + i];
            }

        }

        internal static void CopyCastBuffer(sbyte[] buffer, int offset, int len, byte[] targetBuffer, int targetOffset)
        {
            if (offset < 0 || len < 0 || offset + len > buffer.Length || targetOffset < 0 || targetOffset + len > targetBuffer.Length) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < len; i++)
            {
                targetBuffer[i + targetOffset] = (byte)buffer[offset + i];
            }

        }

        public static sbyte ByteValue(this int? value)
        {
            return (sbyte)value.Value;
        }

        public static int IntValue(this int value)
        {
            return value;
        }

        public static long LongValue(this long value)
        {
            return value;
        }

        public static double DoubleValue(this double value)
        {
            return value;
        }

        public static void Print(this TextWriter writer, object value)
        {
            writer.Write(value);
        }

        public static void Println(this TextWriter writer)
        {
            writer.WriteLine();
        }

        public static void Println(this TextWriter writer, object value)
        {
            writer.WriteLine(value);
        }

        public static void Printf(this TextWriter writer, string format, params object[] args)
        {
            //  same call in Java
            Format(writer, format, args);
        }

        public static void Format(this TextWriter writer, string format, params object[] args)
        {
            writer.WriteLine(ConvertStringFormat(format), args);
        }

        public static string Substring(this StringBuilder sb, int start, int end)
        {
            return sb.ToString().Substring(start, end - start + 1);
        }

        public static string StringFormat(string format, params object[] args)
        {
            return string.Format(ConvertStringFormat(format), args);
        }

        /// <summary>
        /// Partial suport for converting java string format to C# string format
        /// </summary>
        /// <param name="format">Java format string</param>
        /// <returns>C# format string</returns>
        internal static string ConvertStringFormat(string format)
        {
            if (string.IsNullOrEmpty(format) || !StringSplitter.IsMatch(format))
            {
                return format;
            }

            string[] parts = GetFormatParts(format);
            ConvertParts(parts);
            return string.Join("", parts);
        }

        private static string[] GetFormatParts(string format)
        {
            return StringSplitter.Split(format);
        }

        private static void ConvertParts(string[] parts)
        {
            int index = 0;

            for(int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (string.IsNullOrWhiteSpace(part))
                {
                    continue;
                }

                if (StringSplitter.Match(part).Success)
                {
                    var formatparts = FormatSplitter.Split(part);
                    var flags = formatparts[1];
                    var width = formatparts[2];
                    var conversion = formatparts[3];

                    var formatSetting = new FormatSetting
                        {
                            Index = index
                        };

                    if (!string.IsNullOrWhiteSpace(flags))
                    {
                        ParseFlags(formatSetting, flags);
                    }

                    if (!string.IsNullOrWhiteSpace(width))
                    {
                        ParseWidth(formatSetting, width);
                    }

                    formatSetting.SetConversion(conversion[0]);

                    parts[i] = formatSetting.ToString();

                    index++;
                }
            }
        }

        private static void ParseFlags(FormatSetting formatSetting, string flags)
        {
            foreach (var flag in flags)
            {
                if (flag == '0')
                {
                    formatSetting.IsZeroPadded = true;
                    continue;
                }

                if (flag == ',')
                {
                    formatSetting.HasLocalSpesificSeparator = true;
                    continue;
                }

                throw new NotSupportedException();
            }
        }

        private static void ParseWidth(FormatSetting formatSetting, string width)
        {
            if (string.IsNullOrEmpty(width))
            {
                return;
            }

            if (!width.Contains("."))
            {
                formatSetting.IsWidthSpecified = true;
                formatSetting.IntegerWidth = int.Parse(width);
                return;
            }
            if (width.StartsWith("."))
            {
                formatSetting.FractionWidth = int.Parse(width.Substring(1));
            }
            else
            {
                string[] parts = width.Split(".");
                formatSetting.IntegerWidth = int.Parse(parts[0]);
                formatSetting.FractionWidth = int.Parse(parts[1]);
                formatSetting.IsWidthSpecified = true;
            }
            formatSetting.IsFractionWidthSpecified = true;
        }

        private class FormatSetting
        {
            private char _conversion;

            public int Index { get; set; }

            public bool IsZeroPadded { get; set; }

            public bool HasLocalSpesificSeparator { get; set; }

            public bool IsWidthSpecified { get; set; }

            public int IntegerWidth { get; set; }

            public bool IsFractionWidthSpecified { get; set; }

            public int FractionWidth { get; set; }

            public void SetConversion(char conversion)
            {
                _conversion = conversion;
            }

            public override string ToString()
            {
                string format = "";
                switch (Char.ToLower(_conversion))
                {
                    case 'd':
                    case 'f':
                    case 's':
                        break;

                    case 'x':
                        format = "x";
                        break;

                    case 'n':
                        return Environment.NewLine;

                    default:
                        throw new NotSupportedException();
                }

                var result = "{" + Index +":";

                if (!string.IsNullOrEmpty(format))
                {
                    result += format;
                    if (IsWidthSpecified)
                    {
                        result += IntegerWidth;
                        if (IsFractionWidthSpecified)
                        {
                            throw new NotSupportedException();
                        }
                    }
                    else if (IsFractionWidthSpecified)
                    {
                        result += FractionWidth;
                    }
                }
                else
                {
                    if (IsWidthSpecified)
                        result += new String('0', IntegerWidth);

                    if (IsFractionWidthSpecified)
                    {
                        result += "." + new String('#', FractionWidth);
                    }
                }

                return result + "}";
            }
        }
    }
}
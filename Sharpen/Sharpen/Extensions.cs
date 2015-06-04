using System.Reflection;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Specialized;
using System.Xml;

namespace Sharpen
{
    using ICSharpCode.SharpZipLib.Zip.Compression;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class Extensions
    {
        private static readonly long EPOCH_TICKS;
        
        //  The format specifiers which do not correspond to arguments have the following syntax:
        //  %[flags][width]conversion
        //  http://docs.oracle.com/javase/7/docs/api/java/util/Formatter.html
        private const string FlagRegexPattern = "[-\\#+\\s0,]*";
        private const string WidthRegexPattern = "\\d*\\.*\\d*";
        private const string ConversionRegexPattern = "(?i:[sdnfx]{1})";
        private static readonly Regex _stringSplitter = new Regex("(%" + FlagRegexPattern + WidthRegexPattern + ConversionRegexPattern + ")", RegexOptions.Compiled);
        private static readonly Regex _formatSplitter = new Regex("("+FlagRegexPattern+")("+WidthRegexPattern+")("+ConversionRegexPattern+")", RegexOptions.Compiled);

        static Extensions()
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            EPOCH_TICKS = time.Ticks;
        }

        public static void Add(this IList list, int index, object item)
        {
            list.Insert(index, item);
        }
        
        public static void Add<T>(this IList<T> list, int index, T item)
        {
            list.Insert(index, item);
        }

        public static void AddFirst<T>(this IList<T> list, T item)
        {
            list.Insert(0, item);
        }

        public static void AddLast<T>(this IList<T> list, T item)
        {
            list.Add(item);
        }

        public static void RemoveLast<T>(this IList<T> list)
        {
            if (list.Count > 0)
                list.Remove(list.Count - 1);
        }

        public static StringBuilder AppendRange(this StringBuilder sb, string str, int start, int end)
        {
            return sb.Append(str, start, end - start);
        }

        public static StringBuilder Delete(this StringBuilder sb, int start, int end)
        {
            return sb.Remove(start, end - start);
        }

        public static void SetCharAt(this StringBuilder sb, int index, char c)
        {
            sb[index] = c;
        }

        public static int IndexOf(this StringBuilder sb, string str)
        {
            return sb.ToString().IndexOf(str);
        }

        public static Iterable<T> AsIterable<T>(this IEnumerable<T> s)
        {
            return new EnumerableWrapper<T>(s);
        }

        public static int BitCount(int val)
        {
            uint num = (uint) val;
            int count = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((num & 1) != 0)
                {
                    count++;
                }
                num >>= 1;
            }
            return count;
        }

        public static IndexOutOfRangeException CreateIndexOutOfRangeException(int index)
        {
            return new IndexOutOfRangeException("Index: " + index);
        }

        public static CultureInfo CreateLocale(string language, string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                return CultureInfo.GetCultureInfoByIetfLanguageTag(language);
            }

            return CultureInfo.GetCultureInfo(string.Format("{0}-{1}", language, country));
        }

        public static CultureInfo CreateLocale(string language, string country, string variant)
        {
            return CultureInfo.GetCultureInfo("en-US");
        }

        public static string Name(this Encoding e)
        {
            return e.BodyName.ToUpper();
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

        public static ByteBuffer Encode(this Encoding e, CharSequence str)
        {
            return ByteBuffer.Wrap(e.GetBytes(str.ToString()));
        }

        public static ByteBuffer Encode(this Encoding e, string str)
        {
            return ByteBuffer.Wrap(e.GetBytes(str));
        }

        private static UTF8Encoding UTF8Encoder = new UTF8Encoding(false, true);

        public static Encoding GetEncoding(string name)
        {
//			Encoding e = Encoding.GetEncoding (name, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            try
            {
                Encoding e = Encoding.GetEncoding(name.Replace('_', '-'));
                if (e is UTF8Encoding)
                    return UTF8Encoder;
                return e;
            }
            catch (ArgumentException)
            {
                throw new UnsupportedCharsetException(name);
            }
        }

        public static ICollection<KeyValuePair<T, U>> EntrySet<T, U>(this IDictionary<T, U> s)
        {
            return s;
        }

        public static void Finish(this Inflater i)
        {
        }

        public static bool AddItem<T>(this IList<T> list, T item)
        {
            list.Add(item);
            return true;
        }

        public static bool AddItem<T>(this ICollection<T> list, T item)
        {
            list.Add(item);
            return true;
        }

        public static bool ContainsKey(this IDictionary d, object key)
        {
            return d.Contains(key);
        }

        public static U Get<T, U>(this IDictionary<T, U> d, T key)
        {
            U val;
            d.TryGetValue(key, out val);
            return val;
        }
        
        public static object Get(this IDictionary d, object key)
        {
            return d[key];
        }

        public static U Put<T, U>(this IDictionary<T, U> d, T key, U value)
        {
            U old;
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

        public static void PutAll<T, U>(this IDictionary<T, U> d, IDictionary<T, U> values)
        {
            foreach (KeyValuePair<T, U> val in values)
                d[val.Key] = val.Value;
        }

        public static object Put(this Hashtable d, object key, object value)
        {
            object old = d[key];
            d[key] = value;
            return old;
        }

        public static string Put(this StringDictionary d, string key, string value)
        {
            string old = d[key];
            d[key] = value;
            return old;
        }

        public static CultureInfo GetEnglishCulture()
        {
            return CultureInfo.GetCultureInfo("en-US");
        }

        public static T GetFirst<T>(this IList<T> list)
        {
            return ((list.Count == 0) ? default(T) : list[0]);
        }

        public static CultureInfo GetGermanCulture()
        {
            CultureInfo r = CultureInfo.GetCultureInfo("de-DE");
            return r;
        }

        public static T GetLast<T>(this IList<T> list)
        {
            return ((list.Count == 0) ? default(T) : list[list.Count - 1]);
        }

        public static int GetOffset(this TimeZoneInfo tzone, long date)
        {
            return (int) tzone.GetUtcOffset(MillisToDateTimeOffset(date, 0).DateTime).TotalMilliseconds;
        }

        public static InputStream GetResourceAsStream(this Type type, string name)
        {
            string str2 = type.Assembly.GetName().Name + ".resources";
            string[] textArray1 = new string[] {str2, ".", type.Namespace, ".", name};
            string str = string.Concat(textArray1);
            Stream manifestResourceStream = type.Assembly.GetManifestResourceStream(str);
            if (manifestResourceStream == null)
            {
                return null;
            }
            return InputStream.Wrap(manifestResourceStream);
        }

        public static long GetTime(this DateTime dateTime)
        {
            return
                new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeSpan.Zero)
                    .ToMillisecondsSinceEpoch();
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

        public static void InitCause(this Exception ex, Exception cause)
        {
            Console.WriteLine(cause);
        }

        public static bool IsEmpty(this ICollection col)
        {
            return (col.Count == 0);
        }
        
        public static bool IsEmpty<T>(this ICollection<T> col)
        {
            return (col.Count == 0);
        }

        public static bool IsEmpty<T>(this Stack<T> col)
        {
            return (col.Count == 0);
        }

        public static bool IsLower(this char c)
        {
            return char.IsLower(c);
        }

        public static bool IsUpper(this char c)
        {
            return char.IsUpper(c);
        }

        public static Sharpen.Iterator Iterator(this ICollection col)
        {
            return new EnumeratorWrapper(col, col.GetEnumerator());
        }
        
        public static Sharpen.Iterator<T> Iterator<T>(this ICollection<T> col)
        {
            return new EnumeratorWrapper<T>(col, col.GetEnumerator());
        }

        public static Sharpen.Iterator Iterator(this IEnumerable col)
        {
            return new EnumeratorWrapper(col, col.GetEnumerator());
        }
        
        public static Sharpen.Iterator<T> Iterator<T>(this IEnumerable<T> col)
        {
            return new EnumeratorWrapper<T>(col, col.GetEnumerator());
        }

        public static T Last<T>(this ICollection<T> col)
        {
            IList<T> list = col as IList<T>;
            if (list != null)
            {
                return list[list.Count - 1];
            }
            return col.Last<T>();
        }

        public static ListIterator ListIterator(this IList col)
        {
            return new ListIterator(col);
        }

        public static ListIterator<T> ListIterator<T>(this IList<T> col)
        {
            return new ListIterator<T>(col);
        }

        public static ListIterator<T> ListIterator<T>(this IList<T> col, int n)
        {
            return new ListIterator<T>(col, n);
        }

        public static int LowestOneBit(int val)
        {
            return (((int) 1) << NumberOfTrailingZeros(val));
        }

        public static bool Matches(this string str, string regex)
        {
            Regex regex2 = new Regex(regex);
            return regex2.IsMatch(str);
        }

        public static DateTime CreateDate(long milliSecondsSinceEpoch)
        {
            long num = EPOCH_TICKS + (milliSecondsSinceEpoch*10000);
            return new DateTime(num);
        }

        public static DateTimeOffset MillisToDateTimeOffset(long milliSecondsSinceEpoch, long offsetMinutes)
        {
            TimeSpan offset = TimeSpan.FromMinutes((double) offsetMinutes);
            long num = EPOCH_TICKS + (milliSecondsSinceEpoch*10000);
            return new DateTimeOffset(num + offset.Ticks, offset);
        }

        public static CharsetDecoder NewDecoder(this Encoding enc)
        {
            return new CharsetDecoder(enc);
        }

        public static CharsetEncoder NewEncoder(this Encoding enc)
        {
            return new CharsetEncoder(enc);
        }

        public static int NumberOfLeadingZeros(int val)
        {
            uint num = (uint) val;
            int count = 0;
            while ((num & 0x80000000) == 0)
            {
                num = num << 1;
                count++;
            }
            return count;
        }

        public static int NumberOfTrailingZeros(int val)
        {
            uint num = (uint) val;
            int count = 0;
            while ((num & 1) == 0)
            {
                num = num >> 1;
                count++;
            }
            return count;
        }

        public static int Read(this StreamReader reader, char[] data)
        {
            return reader.Read(data, 0, data.Length);
        }

        public static T Remove<T>(this IList<T> list, T item)
        {
            int index = list.IndexOf(item);
            if (index == -1)
            {
                return default(T);
            }
            T local = list[index];
            list.RemoveAt(index);
            return local;
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

        public static T RemoveFirst<T>(this IList<T> list)
        {
            return list.Remove<T>(0);
        }

        public static string ReplaceAll(this string str, string regex, string replacement)
        {
            Regex rgx = new Regex(regex);

            if (replacement.IndexOfAny(new char[] {'\\', '$'}) != -1)
            {
                // Back references not yet supported
                StringBuilder sb = new StringBuilder();
                for (int n = 0; n < replacement.Length; n++)
                {
                    char c = replacement[n];
                    if (c == '$')
                        throw new NotSupportedException("Back references not supported");
                    if (c == '\\')
                        c = replacement[++n];
                    sb.Append(c);
                }
                replacement = sb.ToString();
            }

            return rgx.Replace(str, replacement);
        }

        public static bool RegionMatches(this string str, bool ignoreCase, int toOffset, string other, int ooffset,
                                         int len)
        {
            if (toOffset < 0 || ooffset < 0 || toOffset + len > str.Length || ooffset + len > other.Length)
                return false;
            return string.Compare(str, toOffset, other, ooffset, len) == 0;
        }

        public static object Set(this IList list, int index, object item)
        {
            object old = list[index];
            list[index] = item;
            return old;
        }
        
        public static T Set<T>(this IList<T> list, int index, T item)
        {
            T old = list[index];
            list[index] = item;
            return old;
        }

        public static int Signum(long val)
        {
            if (val < 0)
            {
                return -1;
            }
            if (val > 0)
            {
                return 1;
            }
            return 0;
        }

        public static void RemoveAll<T, U>(this ICollection<T> col, ICollection<U> items) where U : T
        {
            foreach (var u in items)
                col.Remove(u);
        }

        public static bool ContainsAll<T, U>(this ICollection<T> col, ICollection<U> items) where U : T
        {
            foreach (var u in items)
                if (!col.Any(n => (object.ReferenceEquals(n, u)) || n.Equals(u)))
                    return false;
            return true;
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

        public static IList<T> SubList<T>(this IList<T> list, int start, int len)
        {
            List<T> sublist = new List<T>(len);
            for (int i = start; i < (start + len); i++)
            {
                sublist.Add(list[i]);
            }
            return sublist;
        }

        public static char[] ToCharArray(this string str)
        {
            char[] destination = new char[str.Length];
            str.CopyTo(0, destination, 0, str.Length);
            return destination;
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
            return (((dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks) - EPOCH_TICKS)/TimeSpan.TicksPerMillisecond);
        }

        public static string ToOctalString(int val)
        {
            return Convert.ToString(val, 8);
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

        public static string ToString(int val, int bas)
        {
            return Convert.ToString(val, bas);
        }

        public static IList<U> UpcastTo<T, U>(this IList<T> s) where T : U
        {
            List<U> list = new List<U>(s.Count);
            for (int i = 0; i < s.Count; i++)
            {
                list.Add((U) s[i]);
            }
            return list;
        }

        public static ICollection<U> UpcastTo<T, U>(this ICollection<T> s) where T : U
        {
            List<U> list = new List<U>(s.Count);
            foreach (var v in s)
            {
                list.Add((U) v);
            }
            return list;
        }

        public static T ValueOf<T>(T val)
        {
            return val;
        }

        public static int ValueOf(string val)
        {
            return Convert.ToInt32(val);
        }

        public static int GetTotalInFixed(this Inflater inf)
        {
            if (inf.TotalIn > 0)
                return Convert.ToInt32(inf.TotalIn) + 4;
            else
                return 0;
        }

        public static int GetRemainingInputFixed(this Inflater inf)
        {
            if (inf.RemainingInput >= 4)
                return inf.RemainingInput - 4;
            else
                return 0;
        }

        public static string GetTestName(object obj)
        {
            return GetTestName();
        }

        public static string GetTestName()
        {
            MethodBase met;
            int n = 0;
            do
            {
                met = new StackFrame(n).GetMethod();
                if (met != null)
                {
                    foreach (Attribute at in met.GetCustomAttributes(true))
                    {
                        if (at.GetType().FullName == "NUnit.Framework.TestAttribute")
                        {
                            // Convert back to camel case
                            string name = met.Name;
                            if (char.IsUpper(name[0]))
                                name = char.ToLower(name[0]) + name.Substring(1);
                            return name;
                        }
                    }
                }
                n++;
            } while (met != null);
            return "";
        }

        public static string GetHostAddress(this IPAddress addr)
        {
            return addr.ToString();
        }

        public static IPAddress GetAddressByName(string name)
        {
            if (name == "0.0.0.0")
                return IPAddress.Any;
            return Dns.GetHostAddresses(name).FirstOrDefault();
        }

        public static string GetImplementationVersion(this System.Reflection.Assembly asm)
        {
            return asm.GetName().Version.ToString();
        }

        public static string GetHost(this Uri uri)
        {
            return string.IsNullOrEmpty(uri.Host) ? null : uri.Host;
        }

        public static string GetUserInfo(this Uri uri)
        {
            return string.IsNullOrEmpty(uri.UserInfo) ? null : uri.UserInfo;
        }

        public static string GetQuery(this Uri uri)
        {
            return string.IsNullOrEmpty(uri.Query) ? null : uri.Query;
        }

        public static HttpURLConnection OpenConnection(this Uri uri)
        {
            return new HttpsURLConnection(uri);
        }

        public static HttpURLConnection OpenConnection(this Uri uri, Proxy p)
        {
            return new HttpsURLConnection(uri, p);
        }

        public static Uri ToURI(this Uri uri)
        {
            return uri;
        }

        public static Uri ToURL(this Uri uri)
        {
            return uri;
        }

        public static InputStream GetInputStream(this Socket socket)
        {
            return new System.Net.Sockets.NetworkStream(socket);
        }

        public static OutputStream GetOutputStream(this Socket socket)
        {
            return new System.Net.Sockets.NetworkStream(socket);
        }

        public static int GetLocalPort(this Socket socket)
        {
            return ((IPEndPoint) socket.LocalEndPoint).Port;
        }

        public static int GetPort(this Socket socket)
        {
            return ((IPEndPoint) socket.RemoteEndPoint).Port;
        }

        public static IPAddress GetInetAddress(this Socket socket)
        {
            return ((IPEndPoint) socket.RemoteEndPoint).Address;
        }

        public static void Bind2(this Socket socket, EndPoint ep)
        {
            if (ep == null)
                socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            else
                socket.Bind(ep);
        }


        public static void Connect(this Socket socket, EndPoint ep, int timeout)
        {
            try
            {
                IAsyncResult res = socket.BeginConnect(ep, null, null);
                if (!res.AsyncWaitHandle.WaitOne(timeout > 0 ? timeout : Timeout.Infinite, true))
                    throw new IOException("Connection timeout");
            }
            catch (SocketException se)
            {
                throw new IOException(se.Message);
            }
        }

        public static Socket CreateServerSocket(int port, int backlog, IPAddress addr)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Bind(new IPEndPoint(addr, port));
            s.Listen(backlog);
            return s;
        }

        public static Socket CreateSocket(string host, int port)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(host, port);
            return s;
        }

        public static Socket CreateSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public static bool RemoveElement(this ArrayList list, object elem)
        {
            int i = list.IndexOf(elem);
            if (i == -1)
                return false;
            else
            {
                list.RemoveAt(i);
                return true;
            }
        }

        public static System.Threading.Semaphore CreateSemaphore(int count)
        {
            return new System.Threading.Semaphore(count, int.MaxValue);
        }

        public static void SetCommand(this ProcessStartInfo si, IList<string> args)
        {
            si.FileName = args[0];
            si.Arguments = string.Join(" ", args.Skip(1).Select(a => "\"" + a + "\"").ToArray());
        }

        public static SystemProcess Start(this ProcessStartInfo si)
        {
            si.UseShellExecute = false;
            si.RedirectStandardInput = true;
            si.RedirectStandardError = true;
            si.RedirectStandardOutput = true;
            si.CreateNoWindow = true;
            return SystemProcess.Start(si);
        }

//        public static Array GetEnumConstants(this Type type)
//        {
//            if (type.IsEnum)
//            {
//                throw new ArgumentException();
//            }
//
//            try
//            {
//                return Enum.GetValues(type);
//            }
//            catch
//            {
//                // just need to match java behaviour
//                return null;
//            }
//        }

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

        public static void Copy(sbyte[] sbuffer, byte[] buffer)
        {
            for (int i = 0; i < sbuffer.Length; i++)
            {
                buffer[i] = (byte)sbuffer[i];
            }
        }

        public static void Copy(byte[] buffer, sbyte[] sbuffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                sbuffer[i] = (sbyte) buffer[i];
            }
        }

        internal static void CopyCastBuffer(byte[] buffer, int offset, int len, sbyte[] target_buffer, int target_offset) 
        {
            if (offset < 0 || len < 0 || offset + len > buffer.Length || target_offset < 0 || target_offset + len > target_buffer.Length) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < len; i++)
            {
                target_buffer[i + target_offset] = (sbyte)buffer[offset + i];
            }
           
        }

        internal static void CopyCastBuffer(sbyte[] buffer, int offset, int len, byte[] target_buffer, int target_offset)
        {
            if (offset < 0 || len < 0 || offset + len > buffer.Length || target_offset < 0 || target_offset + len > target_buffer.Length) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < len; i++)
            {
                target_buffer[i + target_offset] = (byte)buffer[offset + i];
            }

        }
        public static sbyte ByteValue(this int value)
        {
            return (sbyte) value;
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
            if (string.IsNullOrEmpty(format) || !_stringSplitter.IsMatch(format))
            {
                return format;
            }

            string[] parts = GetFormatParts(format);
            ConvertParts(parts);
            return string.Join("", parts);
        }

        private static string[] GetFormatParts(string format)
        {
            return _stringSplitter.Split(format);
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

                if (_stringSplitter.Match(part).Success)
                {
                    var formatparts = _formatSplitter.Split(part);
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using JetBrains.Annotations;

namespace Sharpen
{
    public static class Extensions
    {
        private static readonly long EpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

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
                tzone = "GMT Standard Time";

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(tzone);
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
                        var found = true;
                        for (var i = 0; i < parts.Length; i++)
                            found &= parts[i][0] == tzone[i];

                        if (found)
                            return timezone;
                    }
                }
            }
            var separator = new[] {':'};
            var strArray = tzone.Substring(4).Split(separator);
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
            var t = new TimeSpan(0, hours, minutes, 0, 0);
            if (tzone[3] == '-')
                t = -t;
            return TimeZoneInfo.CreateCustomTimeZone(tzone, t, tzone, tzone);
        }

        public static Iterator<T> Iterator<T>(this IEnumerable<T> col)
        {
            return new EnumeratorWrapper<T>(col, col.GetEnumerator());
        }

        public static DateTime CreateDate(long milliSecondsSinceEpoch)
        {
            return new DateTime(EpochTicks + milliSecondsSinceEpoch*10000);
        }

        public static DateTimeOffset MillisToDateTimeOffset(long milliSecondsSinceEpoch, long offsetMinutes)
        {
            var offset = TimeSpan.FromMinutes(offsetMinutes);
            var num = EpochTicks + (milliSecondsSinceEpoch*10000);
            return new DateTimeOffset(num + offset.Ticks, offset);
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
    }
}
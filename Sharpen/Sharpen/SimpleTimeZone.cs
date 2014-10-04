
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sharpen
{
    public class SimpleTimeZone
    {
        private int _rawOffset;
        private string _ID;

        public static explicit operator TimeZoneInfo(SimpleTimeZone tz)
        {
            TimeZoneInfo result = null;

            if (!string.IsNullOrEmpty(tz._ID))
            {
                result = TimeZoneInfo.FindSystemTimeZoneById(tz._ID);
            }

            if (result != null)
            {
                return result;
            }

            ReadOnlyCollection<TimeZoneInfo> tz1 = TimeZoneInfo.GetSystemTimeZones();

            result = (from t in tz1 where t.BaseUtcOffset.TotalMilliseconds == tz._rawOffset select t).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            throw new NotSupportedException();
        }

        public SimpleTimeZone(int rawOffset, string ID)
        {
            _rawOffset = rawOffset;
            _ID = ID;
        }
    }
}
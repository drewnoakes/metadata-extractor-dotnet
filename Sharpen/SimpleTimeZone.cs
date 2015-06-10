
using System;
using System.Linq;

namespace Sharpen
{
    public class SimpleTimeZone
    {
        private readonly int _rawOffset;
        private readonly string _id;

        public static explicit operator TimeZoneInfo(SimpleTimeZone tz)
        {
            TimeZoneInfo result = null;

            if (!string.IsNullOrEmpty(tz._id))
            {
                result = TimeZoneInfo.FindSystemTimeZoneById(tz._id);
            }

            if (result != null)
            {
                return result;
            }

            var tz1 = TimeZoneInfo.GetSystemTimeZones();

            result = (from t in tz1 where t.BaseUtcOffset.TotalMilliseconds == tz._rawOffset select t).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            throw new NotSupportedException();
        }

        public SimpleTimeZone(int rawOffset, string id)
        {
            _rawOffset = rawOffset;
            _id = id;
        }
    }
}
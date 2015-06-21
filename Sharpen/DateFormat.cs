using System;

namespace Sharpen
{
    public abstract class DateFormat
    {
        public abstract DateTime Parse (string value);

        public void SetTimeZone (TimeZoneInfo timeZone)
        {
        }
    }
}


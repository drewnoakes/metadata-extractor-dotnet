using System;
using System.Globalization;

namespace Sharpen
{
    public enum CalendarEnum
    {
        Year,
        Month,
        MonthOneBased,
        DayOfMonth,
        Hour,
        HourOfDay,
        Minute,
        Second,
        Millisecond
    }

    public abstract class Calendar
    {
        private static readonly TimeZoneInfo DefaultTimeZone = TimeZoneInfo.Local;

        private DateTime m_calendarDate;
        private TimeZoneInfo m_tz;

        protected Calendar(TimeZoneInfo value)
        {
            m_tz = value;
            m_calendarDate = TimeZoneInfo.ConvertTime(DateTime.Now, DefaultTimeZone, m_tz);
        }

        protected Calendar()
        {
            m_tz = DefaultTimeZone;
            m_calendarDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
        }

        protected Calendar(int year, int month, int dayOfMonth)
        {
            m_tz = DefaultTimeZone;
            m_calendarDate = new DateTime(year, month + 1, dayOfMonth);
        }

        protected Calendar(int year, int month, int dayOfMonth, int hourOfDay, int minute, int second)
        {
            m_tz = DefaultTimeZone;
            bool addDay = false;
            if (hourOfDay == 24)
            {
                hourOfDay = 0;
                addDay = true;
            }
            m_calendarDate = new DateTime(year, month + 1, dayOfMonth, hourOfDay, minute, second);
            if (addDay)
            {
                m_calendarDate = m_calendarDate.AddDays(1);
            }
        }

        public long GetTimeInMillis()
        {
            return GetTime().Ticks / TimeSpan.TicksPerMillisecond;
        }

        public void SetTimeInMillis(long millis)
        {
            var ticks = millis * TimeSpan.TicksPerMillisecond;
            m_calendarDate = new DateTime(ticks, DateTimeKind.Unspecified);
        }

        public DateTime GetTime()
        {
            return TimeZoneInfo.ConvertTime(m_calendarDate, m_tz, DefaultTimeZone);
        }

        public void SetTime(DateTime date)
        {
            m_calendarDate = date;
        }

        public TimeZoneInfo GetTimeZone()
        {
            return m_tz;
        }

        public void SetTimeZone(TimeZoneInfo value)
        {
            m_tz = value;
        }

        public int Get(CalendarEnum field)
        {
            switch (field)
            {
                case CalendarEnum.Year:
                    return m_calendarDate.Year;

                case CalendarEnum.Month:
                    return m_calendarDate.Month - 1;

                case CalendarEnum.MonthOneBased:
                    return m_calendarDate.Month;

                case CalendarEnum.DayOfMonth:
                    return m_calendarDate.Day;

                case CalendarEnum.Hour:
                case CalendarEnum.HourOfDay:
                    return m_calendarDate.Hour;

                case CalendarEnum.Minute:
                    return m_calendarDate.Minute;

                case CalendarEnum.Second:
                    return m_calendarDate.Second;

                case CalendarEnum.Millisecond:
                    return m_calendarDate.Millisecond;
            }

            throw new NotSupportedException();
        }

        public void Set(CalendarEnum field, int value)
        {
            var max = GetMaximum(field);
            var mod = max + 1;

            switch (field)
            {
                case CalendarEnum.Year:
                    //  to avoid exception which is absent in java Calendar
                    value = value % mod;
                    m_calendarDate = m_calendarDate.AddYears(value - m_calendarDate.Year);
                    return;

                case CalendarEnum.Month:
                    m_calendarDate = m_calendarDate.AddMonths((value + 1) - m_calendarDate.Month);
                    return;

                case CalendarEnum.MonthOneBased:
                    m_calendarDate = m_calendarDate.AddMonths(value - m_calendarDate.Month);
                    return;

                case CalendarEnum.DayOfMonth:
                    m_calendarDate = m_calendarDate.AddDays(value - m_calendarDate.Day);
                    return;

                case CalendarEnum.Hour:
                    m_calendarDate = m_calendarDate.AddHours(value - m_calendarDate.Hour);
                    return;

                case CalendarEnum.HourOfDay:
                    //  hour of day has max value == 24, which means next day
                    if (value == 24)
                    {
                        Set(CalendarEnum.Hour, 0);
                        m_calendarDate = m_calendarDate.AddDays(1);
                    }
                    else
                    {
                        Set(CalendarEnum.Hour, value);
                    }
                    return;

                case CalendarEnum.Minute:
                    m_calendarDate = m_calendarDate.AddMinutes(value - m_calendarDate.Minute);
                    return;

                case CalendarEnum.Second:
                    m_calendarDate = m_calendarDate.AddSeconds(value - m_calendarDate.Second);
                    return;

                case CalendarEnum.Millisecond:
                    //m_calendarDate = m_calendarDate.AddMilliseconds(value - m_calendarDate.Millisecond);
                    //  this not works right, because of miliseconds fraction, so we need to go another way here
                    m_calendarDate = new DateTime(m_calendarDate.Year, m_calendarDate.Month, m_calendarDate.Day,
                                                  m_calendarDate.Hour,
                                                  m_calendarDate.Minute, m_calendarDate.Second, value,
                                                  m_calendarDate.Kind);
                    return;
            }

            throw new NotSupportedException();
        }

        public abstract int GetMaximum(CalendarEnum field);

        public void Set(int year, int month, int day, int hourOfDay, int minute, int second)
        {
            Set(CalendarEnum.Year, year);
            Set(CalendarEnum.Month, month);
            Set(CalendarEnum.DayOfMonth, day);
            Set(CalendarEnum.HourOfDay, hourOfDay);
            Set(CalendarEnum.Minute, minute);
            Set(CalendarEnum.Second, second);
            Set(CalendarEnum.Millisecond, 0);
        }

        public static Calendar GetInstance(CultureInfo culture)
        {
            return new GregorianCalendar();
        }

        public static Calendar GetInstance(TimeZoneInfo value)
        {
            return new GregorianCalendar(value);
        }
    }
}
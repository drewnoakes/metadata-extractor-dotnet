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

        private DateTime _mCalendarDate;
        private TimeZoneInfo _mTz;

        protected Calendar(TimeZoneInfo value)
        {
            _mTz = value;
            _mCalendarDate = TimeZoneInfo.ConvertTime(DateTime.Now, DefaultTimeZone, _mTz);
        }

        protected Calendar()
        {
            _mTz = DefaultTimeZone;
            _mCalendarDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
        }

        protected Calendar(int year, int month, int dayOfMonth)
        {
            _mTz = DefaultTimeZone;
            _mCalendarDate = new DateTime(year, month + 1, dayOfMonth);
        }

        protected Calendar(int year, int month, int dayOfMonth, int hourOfDay, int minute, int second)
        {
            _mTz = DefaultTimeZone;
            bool addDay = false;
            if (hourOfDay == 24)
            {
                hourOfDay = 0;
                addDay = true;
            }
            _mCalendarDate = new DateTime(year, month + 1, dayOfMonth, hourOfDay, minute, second);
            if (addDay)
            {
                _mCalendarDate = _mCalendarDate.AddDays(1);
            }
        }

        public long GetTimeInMillis()
        {
            return GetTime().Ticks / TimeSpan.TicksPerMillisecond;
        }

        public void SetTimeInMillis(long millis)
        {
            var ticks = millis * TimeSpan.TicksPerMillisecond;
            _mCalendarDate = new DateTime(ticks, DateTimeKind.Unspecified);
        }

        public DateTime GetTime()
        {
            return TimeZoneInfo.ConvertTime(_mCalendarDate, _mTz, DefaultTimeZone);
        }

        public void SetTime(DateTime date)
        {
            _mCalendarDate = date;
        }

        public TimeZoneInfo GetTimeZone()
        {
            return _mTz;
        }

        public void SetTimeZone(TimeZoneInfo value)
        {
            _mTz = value;
        }

        public int Get(CalendarEnum field)
        {
            switch (field)
            {
                case CalendarEnum.Year:
                    return _mCalendarDate.Year;

                case CalendarEnum.Month:
                    return _mCalendarDate.Month - 1;

                case CalendarEnum.MonthOneBased:
                    return _mCalendarDate.Month;

                case CalendarEnum.DayOfMonth:
                    return _mCalendarDate.Day;

                case CalendarEnum.Hour:
                case CalendarEnum.HourOfDay:
                    return _mCalendarDate.Hour;

                case CalendarEnum.Minute:
                    return _mCalendarDate.Minute;

                case CalendarEnum.Second:
                    return _mCalendarDate.Second;

                case CalendarEnum.Millisecond:
                    return _mCalendarDate.Millisecond;
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
                    _mCalendarDate = _mCalendarDate.AddYears(value - _mCalendarDate.Year);
                    return;

                case CalendarEnum.Month:
                    _mCalendarDate = _mCalendarDate.AddMonths((value + 1) - _mCalendarDate.Month);
                    return;

                case CalendarEnum.MonthOneBased:
                    _mCalendarDate = _mCalendarDate.AddMonths(value - _mCalendarDate.Month);
                    return;

                case CalendarEnum.DayOfMonth:
                    _mCalendarDate = _mCalendarDate.AddDays(value - _mCalendarDate.Day);
                    return;

                case CalendarEnum.Hour:
                    _mCalendarDate = _mCalendarDate.AddHours(value - _mCalendarDate.Hour);
                    return;

                case CalendarEnum.HourOfDay:
                    //  hour of day has max value == 24, which means next day
                    if (value == 24)
                    {
                        Set(CalendarEnum.Hour, 0);
                        _mCalendarDate = _mCalendarDate.AddDays(1);
                    }
                    else
                    {
                        Set(CalendarEnum.Hour, value);
                    }
                    return;

                case CalendarEnum.Minute:
                    _mCalendarDate = _mCalendarDate.AddMinutes(value - _mCalendarDate.Minute);
                    return;

                case CalendarEnum.Second:
                    _mCalendarDate = _mCalendarDate.AddSeconds(value - _mCalendarDate.Second);
                    return;

                case CalendarEnum.Millisecond:
                    //m_calendarDate = m_calendarDate.AddMilliseconds(value - m_calendarDate.Millisecond);
                    //  this not works right, because of miliseconds fraction, so we need to go another way here
                    _mCalendarDate = new DateTime(_mCalendarDate.Year, _mCalendarDate.Month, _mCalendarDate.Day,
                                                  _mCalendarDate.Hour,
                                                  _mCalendarDate.Minute, _mCalendarDate.Second, value,
                                                  _mCalendarDate.Kind);
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
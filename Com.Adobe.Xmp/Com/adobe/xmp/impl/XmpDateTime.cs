// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;
using System.Globalization;
using Sharpen;
using Calendar = Sharpen.Calendar;
using GregorianCalendar = Sharpen.GregorianCalendar;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>The implementation of <c>XMPDateTime</c>.</summary>
    /// <remarks>
    /// The implementation of <c>XMPDateTime</c>. Internally a <c>calendar</c> is used
    /// plus an additional nano seconds field, because <c>Calendar</c> supports only milli
    /// seconds. The <c>nanoSeconds</c> convers only the resolution beyond a milli second.
    /// </remarks>
    /// <since>16.02.2006</since>
    public sealed class XmpDateTime : IXmpDateTime
    {
        private int _year;

        private int _month;

        private int _day;

        private int _hour;

        private int _minute;

        private int _second;

        /// <summary>Use NO time zone as default</summary>
        private TimeZoneInfo _timeZone;

        /// <summary>The nano seconds take micro and nano seconds, while the milli seconds are in the calendar.</summary>
        private int _nanoSeconds;

        private bool _hasDate;

        private bool _hasTime;

        private bool _hasTimeZone;

        /// <summary>
        /// Creates an <c>XMPDateTime</c>-instance with the current time in the default time
        /// zone.
        /// </summary>
        public XmpDateTime()
        {
        }

        /// <summary>Creates an <c>XMPDateTime</c>-instance from a calendar.</summary>
        /// <param name="calendar">a <c>Calendar</c></param>
        public XmpDateTime(Calendar calendar)
        {
            // extract the date and timezone from the calendar provided
            DateTime date = calendar.GetTime();
            TimeZoneInfo zone = calendar.GetTimeZone();
            // put that date into a calendar the pretty much represents ISO8601
            // I use US because it is close to the "locale" for the ISO8601 spec
            GregorianCalendar intCalendar = (GregorianCalendar)Calendar.GetInstance(CultureInfo.InvariantCulture);
            intCalendar.SetGregorianChange(Extensions.CreateDate(long.MinValue));
            intCalendar.SetTimeZone(zone);
            intCalendar.SetTime(date);
            _year = intCalendar.Get(CalendarEnum.Year);
            _month = intCalendar.Get(CalendarEnum.Month) + 1;
            // cal is from 0..12
            _day = intCalendar.Get(CalendarEnum.DayOfMonth);
            _hour = intCalendar.Get(CalendarEnum.HourOfDay);
            _minute = intCalendar.Get(CalendarEnum.Minute);
            _second = intCalendar.Get(CalendarEnum.Second);
            _nanoSeconds = intCalendar.Get(CalendarEnum.Millisecond) * 1000000;
            _timeZone = intCalendar.GetTimeZone();
            // object contains all date components
            _hasDate = _hasTime = _hasTimeZone = true;
        }

        /// <summary>
        /// Creates an <c>XMPDateTime</c>-instance from
        /// a <c>Date</c> and a <c>TimeZone</c>.
        /// </summary>
        /// <param name="date">a date describing an absolute point in time</param>
        /// <param name="timeZone">a TimeZone how to interpret the date</param>
        public XmpDateTime(DateTime date, TimeZoneInfo timeZone)
        {
            GregorianCalendar calendar = new GregorianCalendar(timeZone);
            calendar.SetTime(date);
            _year = calendar.Get(CalendarEnum.Year);
            _month = calendar.Get(CalendarEnum.Month) + 1;
            // cal is from 0..12
            _day = calendar.Get(CalendarEnum.DayOfMonth);
            _hour = calendar.Get(CalendarEnum.HourOfDay);
            _minute = calendar.Get(CalendarEnum.Minute);
            _second = calendar.Get(CalendarEnum.Second);
            _nanoSeconds = calendar.Get(CalendarEnum.Millisecond) * 1000000;
            _timeZone = timeZone;
            // object contains all date components
            _hasDate = _hasTime = _hasTimeZone = true;
        }

        /// <summary>Creates an <c>XMPDateTime</c>-instance from an ISO 8601 string.</summary>
        /// <param name="strValue">an ISO 8601 string</param>
        /// <exception cref="XmpException">If the string is a non-conform ISO 8601 string, an exception is thrown</exception>
        public XmpDateTime(string strValue)
        {
            Iso8601Converter.Parse(strValue, this);
        }

        /// <seealso cref="IXmpDateTime.GetYear()"/>
        public int GetYear()
        {
            return _year;
        }

        /// <seealso cref="IXmpDateTime.SetYear(int)"/>
        public void SetYear(int year)
        {
            _year = Math.Min(Math.Abs(year), 9999);
            _hasDate = true;
        }

        /// <seealso cref="IXmpDateTime.GetMonth()"/>
        public int GetMonth()
        {
            return _month;
        }

        /// <seealso cref="IXmpDateTime.SetMonth(int)"/>
        public void SetMonth(int month)
        {
            if (month < 1)
            {
                _month = 1;
            }
            else
            {
                if (month > 12)
                {
                    _month = 12;
                }
                else
                {
                    _month = month;
                }
            }
            _hasDate = true;
        }

        /// <seealso cref="IXmpDateTime.GetDay()"/>
        public int GetDay()
        {
            return _day;
        }

        /// <seealso cref="IXmpDateTime.SetDay(int)"/>
        public void SetDay(int day)
        {
            if (day < 1)
            {
                _day = 1;
            }
            else
            {
                if (day > 31)
                {
                    _day = 31;
                }
                else
                {
                    _day = day;
                }
            }
            _hasDate = true;
        }

        /// <seealso cref="IXmpDateTime.GetHour()"/>
        public int GetHour()
        {
            return _hour;
        }

        /// <seealso cref="IXmpDateTime.SetHour(int)"/>
        public void SetHour(int hour)
        {
            _hour = Math.Min(Math.Abs(hour), 23);
            _hasTime = true;
        }

        /// <seealso cref="IXmpDateTime.GetMinute()"/>
        public int GetMinute()
        {
            return _minute;
        }

        /// <seealso cref="IXmpDateTime.SetMinute(int)"/>
        public void SetMinute(int minute)
        {
            _minute = Math.Min(Math.Abs(minute), 59);
            _hasTime = true;
        }

        /// <seealso cref="IXmpDateTime.GetSecond()"/>
        public int GetSecond()
        {
            return _second;
        }

        /// <seealso cref="IXmpDateTime.SetSecond(int)"/>
        public void SetSecond(int second)
        {
            _second = Math.Min(Math.Abs(second), 59);
            _hasTime = true;
        }

        /// <seealso cref="IXmpDateTime.GetNanoSecond()"/>
        public int GetNanoSecond()
        {
            return _nanoSeconds;
        }

        /// <seealso cref="IXmpDateTime.SetNanoSecond(int)"/>
        public void SetNanoSecond(int nanoSecond)
        {
            _nanoSeconds = nanoSecond;
            _hasTime = true;
        }

        /// <seealso cref="System.IComparable{T}.CompareTo(object)"/>
        public int CompareTo(object dt)
        {
            long d = GetCalendar().GetTimeInMillis() - ((IXmpDateTime)dt).GetCalendar().GetTimeInMillis();
            if (d != 0)
            {
                return Math.Sign(d);
            }
            // if millis are equal, compare nanoseconds
            d = _nanoSeconds - ((IXmpDateTime)dt).GetNanoSecond();
            return Math.Sign(d);
        }

        /// <seealso cref="IXmpDateTime.GetTimeZone()"/>
        public TimeZoneInfo GetTimeZone()
        {
            return _timeZone;
        }

        /// <seealso cref="IXmpDateTime.SetTimeZone(System.TimeZoneInfo)"/>
        public void SetTimeZone(TimeZoneInfo timeZone)
        {
            _timeZone = timeZone;
            _hasTime = true;
            _hasTimeZone = true;
        }

        /// <seealso cref="IXmpDateTime.HasDate()"/>
        public bool HasDate()
        {
            return _hasDate;
        }

        /// <seealso cref="IXmpDateTime.HasTime()"/>
        public bool HasTime()
        {
            return _hasTime;
        }

        /// <seealso cref="IXmpDateTime.HasTimeZone()"/>
        public bool HasTimeZone()
        {
            return _hasTimeZone;
        }

        /// <seealso cref="IXmpDateTime.GetCalendar()"/>
        public Calendar GetCalendar()
        {
            GregorianCalendar calendar = (GregorianCalendar)Calendar.GetInstance(CultureInfo.InvariantCulture);
            calendar.SetGregorianChange(Extensions.CreateDate(long.MinValue));
            if (_hasTimeZone)
            {
                calendar.SetTimeZone(_timeZone);
            }
            calendar.Set(CalendarEnum.Year, _year);
            calendar.Set(CalendarEnum.Month, _month - 1);
            calendar.Set(CalendarEnum.DayOfMonth, _day);
            calendar.Set(CalendarEnum.HourOfDay, _hour);
            calendar.Set(CalendarEnum.Minute, _minute);
            calendar.Set(CalendarEnum.Second, _second);
            calendar.Set(CalendarEnum.Millisecond, _nanoSeconds / 1000000);
            return calendar;
        }

        public string GetIso8601String()
        {
            return Iso8601Converter.Render(this);
        }

        /// <returns>Returns the ISO string representation.</returns>
        public override string ToString()
        {
            return GetIso8601String();
        }
    }
}

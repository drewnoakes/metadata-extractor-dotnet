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
    /// <summary>The implementation of <code>XMPDateTime</code>.</summary>
    /// <remarks>
    /// The implementation of <code>XMPDateTime</code>. Internally a <code>calendar</code> is used
    /// plus an additional nano seconds field, because <code>Calendar</code> supports only milli
    /// seconds. The <code>nanoSeconds</code> convers only the resolution beyond a milli second.
    /// </remarks>
    /// <since>16.02.2006</since>
    public class XmpDateTime : IXmpDateTime
    {
        private int _year = 0;

        private int _month = 0;

        private int _day = 0;

        private int _hour = 0;

        private int _minute = 0;

        private int _second = 0;

        /// <summary>Use NO time zone as default</summary>
        private TimeZoneInfo _timeZone = null;

        /// <summary>The nano seconds take micro and nano seconds, while the milli seconds are in the calendar.</summary>
        private int _nanoSeconds;

        private bool _hasDate = false;

        private bool _hasTime = false;

        private bool _hasTimeZone = false;

        /// <summary>
        /// Creates an <code>XMPDateTime</code>-instance with the current time in the default time
        /// zone.
        /// </summary>
        public XmpDateTime()
        {
        }

        /// <summary>Creates an <code>XMPDateTime</code>-instance from a calendar.</summary>
        /// <param name="calendar">a <code>Calendar</code></param>
        public XmpDateTime(Calendar calendar)
        {
            // EMPTY
            // extract the date and timezone from the calendar provided
            DateTime date = calendar.GetTime();
            TimeZoneInfo zone = calendar.GetTimeZone();
            // put that date into a calendar the pretty much represents ISO8601
            // I use US because it is close to the "locale" for the ISO8601 spec
            GregorianCalendar intCalendar = (GregorianCalendar)Calendar.GetInstance(CultureInfo.InvariantCulture);
            intCalendar.SetGregorianChange(Extensions.CreateDate(long.MinValue));
            intCalendar.SetTimeZone(zone);
            intCalendar.SetTime(date);
            this._year = intCalendar.Get(CalendarEnum.Year);
            this._month = intCalendar.Get(CalendarEnum.Month) + 1;
            // cal is from 0..12
            this._day = intCalendar.Get(CalendarEnum.DayOfMonth);
            this._hour = intCalendar.Get(CalendarEnum.HourOfDay);
            this._minute = intCalendar.Get(CalendarEnum.Minute);
            this._second = intCalendar.Get(CalendarEnum.Second);
            this._nanoSeconds = intCalendar.Get(CalendarEnum.Millisecond) * 1000000;
            this._timeZone = intCalendar.GetTimeZone();
            // object contains all date components
            _hasDate = _hasTime = _hasTimeZone = true;
        }

        /// <summary>
        /// Creates an <code>XMPDateTime</code>-instance from
        /// a <code>Date</code> and a <code>TimeZone</code>.
        /// </summary>
        /// <param name="date">a date describing an absolute point in time</param>
        /// <param name="timeZone">a TimeZone how to interpret the date</param>
        public XmpDateTime(DateTime date, TimeZoneInfo timeZone)
        {
            GregorianCalendar calendar = new GregorianCalendar(timeZone);
            calendar.SetTime(date);
            this._year = calendar.Get(CalendarEnum.Year);
            this._month = calendar.Get(CalendarEnum.Month) + 1;
            // cal is from 0..12
            this._day = calendar.Get(CalendarEnum.DayOfMonth);
            this._hour = calendar.Get(CalendarEnum.HourOfDay);
            this._minute = calendar.Get(CalendarEnum.Minute);
            this._second = calendar.Get(CalendarEnum.Second);
            this._nanoSeconds = calendar.Get(CalendarEnum.Millisecond) * 1000000;
            this._timeZone = timeZone;
            // object contains all date components
            _hasDate = _hasTime = _hasTimeZone = true;
        }

        /// <summary>Creates an <code>XMPDateTime</code>-instance from an ISO 8601 string.</summary>
        /// <param name="strValue">an ISO 8601 string</param>
        /// <exception cref="XmpException">If the string is a non-conform ISO 8601 string, an exception is thrown</exception>
        public XmpDateTime(string strValue)
        {
            Iso8601Converter.Parse(strValue, this);
        }

        /// <seealso cref="IXmpDateTime.GetYear()"/>
        public virtual int GetYear()
        {
            return _year;
        }

        /// <seealso cref="IXmpDateTime.SetYear(int)"/>
        public virtual void SetYear(int year)
        {
            this._year = Math.Min(Math.Abs(year), 9999);
            this._hasDate = true;
        }

        /// <seealso cref="IXmpDateTime.GetMonth()"/>
        public virtual int GetMonth()
        {
            return _month;
        }

        /// <seealso cref="IXmpDateTime.SetMonth(int)"/>
        public virtual void SetMonth(int month)
        {
            if (month < 1)
            {
                this._month = 1;
            }
            else
            {
                if (month > 12)
                {
                    this._month = 12;
                }
                else
                {
                    this._month = month;
                }
            }
            this._hasDate = true;
        }

        /// <seealso cref="IXmpDateTime.GetDay()"/>
        public virtual int GetDay()
        {
            return _day;
        }

        /// <seealso cref="IXmpDateTime.SetDay(int)"/>
        public virtual void SetDay(int day)
        {
            if (day < 1)
            {
                this._day = 1;
            }
            else
            {
                if (day > 31)
                {
                    this._day = 31;
                }
                else
                {
                    this._day = day;
                }
            }
            this._hasDate = true;
        }

        /// <seealso cref="IXmpDateTime.GetHour()"/>
        public virtual int GetHour()
        {
            return _hour;
        }

        /// <seealso cref="IXmpDateTime.SetHour(int)"/>
        public virtual void SetHour(int hour)
        {
            this._hour = Math.Min(Math.Abs(hour), 23);
            this._hasTime = true;
        }

        /// <seealso cref="IXmpDateTime.GetMinute()"/>
        public virtual int GetMinute()
        {
            return _minute;
        }

        /// <seealso cref="IXmpDateTime.SetMinute(int)"/>
        public virtual void SetMinute(int minute)
        {
            this._minute = Math.Min(Math.Abs(minute), 59);
            this._hasTime = true;
        }

        /// <seealso cref="IXmpDateTime.GetSecond()"/>
        public virtual int GetSecond()
        {
            return _second;
        }

        /// <seealso cref="IXmpDateTime.SetSecond(int)"/>
        public virtual void SetSecond(int second)
        {
            this._second = Math.Min(Math.Abs(second), 59);
            this._hasTime = true;
        }

        /// <seealso cref="IXmpDateTime.GetNanoSecond()"/>
        public virtual int GetNanoSecond()
        {
            return _nanoSeconds;
        }

        /// <seealso cref="IXmpDateTime.SetNanoSecond(int)"/>
        public virtual void SetNanoSecond(int nanoSecond)
        {
            this._nanoSeconds = nanoSecond;
            this._hasTime = true;
        }

        /// <seealso cref="System.IComparable{T}.CompareTo(object)"/>
        public virtual int CompareTo(object dt)
        {
            long d = GetCalendar().GetTimeInMillis() - ((IXmpDateTime)dt).GetCalendar().GetTimeInMillis();
            if (d != 0)
            {
                return (int)Math.Sign(d);
            }
            else
            {
                // if millis are equal, compare nanoseconds
                d = _nanoSeconds - ((IXmpDateTime)dt).GetNanoSecond();
                return (int)Math.Sign(d);
            }
        }

        /// <seealso cref="IXmpDateTime.GetTimeZone()"/>
        public virtual TimeZoneInfo GetTimeZone()
        {
            return _timeZone;
        }

        /// <seealso cref="IXmpDateTime.SetTimeZone(System.TimeZoneInfo)"/>
        public virtual void SetTimeZone(TimeZoneInfo timeZone)
        {
            this._timeZone = timeZone;
            this._hasTime = true;
            this._hasTimeZone = true;
        }

        /// <seealso cref="IXmpDateTime.HasDate()"/>
        public virtual bool HasDate()
        {
            return this._hasDate;
        }

        /// <seealso cref="IXmpDateTime.HasTime()"/>
        public virtual bool HasTime()
        {
            return this._hasTime;
        }

        /// <seealso cref="IXmpDateTime.HasTimeZone()"/>
        public virtual bool HasTimeZone()
        {
            return this._hasTimeZone;
        }

        /// <seealso cref="IXmpDateTime.GetCalendar()"/>
        public virtual Calendar GetCalendar()
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

        /// <sIXmpDateTime.GetIso8601StringO8601String()"/>
        public virtual string GetIso8601String()
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

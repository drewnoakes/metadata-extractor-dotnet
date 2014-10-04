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
using Com.Adobe.Xmp;
using Com.Adobe.Xmp.Impl;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
	/// <summary>The implementation of <code>XMPDateTime</code>.</summary>
	/// <remarks>
	/// The implementation of <code>XMPDateTime</code>. Internally a <code>calendar</code> is used
	/// plus an additional nano seconds field, because <code>Calendar</code> supports only milli
	/// seconds. The <code>nanoSeconds</code> convers only the resolution beyond a milli second.
	/// </remarks>
	/// <since>16.02.2006</since>
	public class XMPDateTimeImpl : XMPDateTime
	{
		private int year = 0;

		private int month = 0;

		private int day = 0;

		private int hour = 0;

		private int minute = 0;

		private int second = 0;

		/// <summary>Use NO time zone as default</summary>
		private TimeZoneInfo timeZone = null;

		/// <summary>The nano seconds take micro and nano seconds, while the milli seconds are in the calendar.</summary>
		private int nanoSeconds;

		private bool hasDate = false;

		private bool hasTime = false;

		private bool hasTimeZone = false;

		/// <summary>
		/// Creates an <code>XMPDateTime</code>-instance with the current time in the default time
		/// zone.
		/// </summary>
		public XMPDateTimeImpl()
		{
		}

		/// <summary>Creates an <code>XMPDateTime</code>-instance from a calendar.</summary>
		/// <param name="calendar">a <code>Calendar</code></param>
		public XMPDateTimeImpl(Sharpen.Calendar calendar)
		{
			// EMPTY
			// extract the date and timezone from the calendar provided
			DateTime date = calendar.GetTime();
			TimeZoneInfo zone = calendar.GetTimeZone();
			// put that date into a calendar the pretty much represents ISO8601
			// I use US because it is close to the "locale" for the ISO8601 spec
			Sharpen.GregorianCalendar intCalendar = (Sharpen.GregorianCalendar)Sharpen.Calendar.GetInstance(CultureInfo.InvariantCulture);
			intCalendar.SetGregorianChange(Sharpen.Extensions.CreateDate(long.MinValue));
			intCalendar.SetTimeZone(zone);
			intCalendar.SetTime(date);
			this.year = intCalendar.Get(Sharpen.CalendarEnum.Year);
			this.month = intCalendar.Get(Sharpen.CalendarEnum.Month) + 1;
			// cal is from 0..12
			this.day = intCalendar.Get(Sharpen.CalendarEnum.DayOfMonth);
			this.hour = intCalendar.Get(Sharpen.CalendarEnum.HourOfDay);
			this.minute = intCalendar.Get(Sharpen.CalendarEnum.Minute);
			this.second = intCalendar.Get(Sharpen.CalendarEnum.Second);
			this.nanoSeconds = intCalendar.Get(Sharpen.CalendarEnum.Millisecond) * 1000000;
			this.timeZone = intCalendar.GetTimeZone();
			// object contains all date components
			hasDate = hasTime = hasTimeZone = true;
		}

		/// <summary>
		/// Creates an <code>XMPDateTime</code>-instance from
		/// a <code>Date</code> and a <code>TimeZone</code>.
		/// </summary>
		/// <param name="date">a date describing an absolute point in time</param>
		/// <param name="timeZone">a TimeZone how to interpret the date</param>
		public XMPDateTimeImpl(DateTime date, TimeZoneInfo timeZone)
		{
			Sharpen.GregorianCalendar calendar = new Sharpen.GregorianCalendar(timeZone);
			calendar.SetTime(date);
			this.year = calendar.Get(Sharpen.CalendarEnum.Year);
			this.month = calendar.Get(Sharpen.CalendarEnum.Month) + 1;
			// cal is from 0..12
			this.day = calendar.Get(Sharpen.CalendarEnum.DayOfMonth);
			this.hour = calendar.Get(Sharpen.CalendarEnum.HourOfDay);
			this.minute = calendar.Get(Sharpen.CalendarEnum.Minute);
			this.second = calendar.Get(Sharpen.CalendarEnum.Second);
			this.nanoSeconds = calendar.Get(Sharpen.CalendarEnum.Millisecond) * 1000000;
			this.timeZone = timeZone;
			// object contains all date components
			hasDate = hasTime = hasTimeZone = true;
		}

		/// <summary>Creates an <code>XMPDateTime</code>-instance from an ISO 8601 string.</summary>
		/// <param name="strValue">an ISO 8601 string</param>
		/// <exception cref="Com.Adobe.Xmp.XMPException">If the string is a non-conform ISO 8601 string, an exception is thrown</exception>
		public XMPDateTimeImpl(string strValue)
		{
			ISO8601Converter.Parse(strValue, this);
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.GetYear()"/>
		public virtual int GetYear()
		{
			return year;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.SetYear(int)"/>
		public virtual void SetYear(int year)
		{
			this.year = Math.Min(Math.Abs(year), 9999);
			this.hasDate = true;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.GetMonth()"/>
		public virtual int GetMonth()
		{
			return month;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.SetMonth(int)"/>
		public virtual void SetMonth(int month)
		{
			if (month < 1)
			{
				this.month = 1;
			}
			else
			{
				if (month > 12)
				{
					this.month = 12;
				}
				else
				{
					this.month = month;
				}
			}
			this.hasDate = true;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.GetDay()"/>
		public virtual int GetDay()
		{
			return day;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.SetDay(int)"/>
		public virtual void SetDay(int day)
		{
			if (day < 1)
			{
				this.day = 1;
			}
			else
			{
				if (day > 31)
				{
					this.day = 31;
				}
				else
				{
					this.day = day;
				}
			}
			this.hasDate = true;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.GetHour()"/>
		public virtual int GetHour()
		{
			return hour;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.SetHour(int)"/>
		public virtual void SetHour(int hour)
		{
			this.hour = Math.Min(Math.Abs(hour), 23);
			this.hasTime = true;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.GetMinute()"/>
		public virtual int GetMinute()
		{
			return minute;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.SetMinute(int)"/>
		public virtual void SetMinute(int minute)
		{
			this.minute = Math.Min(Math.Abs(minute), 59);
			this.hasTime = true;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.GetSecond()"/>
		public virtual int GetSecond()
		{
			return second;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.SetSecond(int)"/>
		public virtual void SetSecond(int second)
		{
			this.second = Math.Min(Math.Abs(second), 59);
			this.hasTime = true;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.GetNanoSecond()"/>
		public virtual int GetNanoSecond()
		{
			return nanoSeconds;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.SetNanoSecond(int)"/>
		public virtual void SetNanoSecond(int nanoSecond)
		{
			this.nanoSeconds = nanoSecond;
			this.hasTime = true;
		}

		/// <seealso cref="System.IComparable{T}.CompareTo(T)"/>
		public virtual int CompareTo(object dt)
		{
			long d = GetCalendar().GetTimeInMillis() - ((XMPDateTime)dt).GetCalendar().GetTimeInMillis();
			if (d != 0)
			{
				return (int)System.Math.Sign(d);
			}
			else
			{
				// if millis are equal, compare nanoseconds
				d = nanoSeconds - ((XMPDateTime)dt).GetNanoSecond();
				return (int)System.Math.Sign(d);
			}
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.GetTimeZone()"/>
		public virtual TimeZoneInfo GetTimeZone()
		{
			return timeZone;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.SetTimeZone(System.TimeZoneInfo)"/>
		public virtual void SetTimeZone(TimeZoneInfo timeZone)
		{
			this.timeZone = timeZone;
			this.hasTime = true;
			this.hasTimeZone = true;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.HasDate()"/>
		public virtual bool HasDate()
		{
			return this.hasDate;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.HasTime()"/>
		public virtual bool HasTime()
		{
			return this.hasTime;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.HasTimeZone()"/>
		public virtual bool HasTimeZone()
		{
			return this.hasTimeZone;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.GetCalendar()"/>
		public virtual Sharpen.Calendar GetCalendar()
		{
			Sharpen.GregorianCalendar calendar = (Sharpen.GregorianCalendar)Sharpen.Calendar.GetInstance(CultureInfo.InvariantCulture);
			calendar.SetGregorianChange(Sharpen.Extensions.CreateDate(long.MinValue));
			if (hasTimeZone)
			{
				calendar.SetTimeZone(timeZone);
			}
			calendar.Set(Sharpen.CalendarEnum.Year, year);
			calendar.Set(Sharpen.CalendarEnum.Month, month - 1);
			calendar.Set(Sharpen.CalendarEnum.DayOfMonth, day);
			calendar.Set(Sharpen.CalendarEnum.HourOfDay, hour);
			calendar.Set(Sharpen.CalendarEnum.Minute, minute);
			calendar.Set(Sharpen.CalendarEnum.Second, second);
			calendar.Set(Sharpen.CalendarEnum.Millisecond, nanoSeconds / 1000000);
			return calendar;
		}

		/// <seealso cref="Com.Adobe.Xmp.XMPDateTime.GetISO8601String()"/>
		public virtual string GetISO8601String()
		{
			return ISO8601Converter.Render(this);
		}

		/// <returns>Returns the ISO string representation.</returns>
		public override string ToString()
		{
			return GetISO8601String();
		}
	}
}

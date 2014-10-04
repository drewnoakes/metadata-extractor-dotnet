// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================
using System;
using Com.Adobe.Xmp;
using Com.Adobe.Xmp.Impl;
using Sharpen;

namespace Com.Adobe.Xmp
{
	/// <summary>
	/// A factory to create <code>XMPDateTime</code>-instances from a <code>Calendar</code> or an
	/// ISO 8601 string or for the current time.
	/// </summary>
	/// <since>16.02.2006</since>
	public sealed class XMPDateTimeFactory
	{
		/// <summary>The UTC TimeZone</summary>
		private static readonly TimeZoneInfo Utc = Sharpen.Extensions.GetTimeZone("UTC");

		/// <summary>Private constructor</summary>
		private XMPDateTimeFactory()
		{
		}

		// EMPTY
		/// <summary>Creates an <code>XMPDateTime</code> from a <code>Calendar</code>-object.</summary>
		/// <param name="calendar">a <code>Calendar</code>-object.</param>
		/// <returns>An <code>XMPDateTime</code>-object.</returns>
		public static XMPDateTime CreateFromCalendar(Sharpen.Calendar calendar)
		{
			return new XMPDateTimeImpl(calendar);
		}

		/// <summary>Creates an empty <code>XMPDateTime</code>-object.</summary>
		/// <returns>Returns an <code>XMPDateTime</code>-object.</returns>
		public static XMPDateTime Create()
		{
			return new XMPDateTimeImpl();
		}

		/// <summary>Creates an <code>XMPDateTime</code>-object from initial values.</summary>
		/// <param name="year">years</param>
		/// <param name="month">
		/// months from 1 to 12<br />
		/// <em>Note:</em> Remember that the month in
		/// <see cref="Sharpen.Calendar"/>
		/// is defined from 0 to 11.
		/// </param>
		/// <param name="day">days</param>
		/// <returns>Returns an <code>XMPDateTime</code>-object.</returns>
		public static XMPDateTime Create(int year, int month, int day)
		{
			XMPDateTime dt = new XMPDateTimeImpl();
			dt.SetYear(year);
			dt.SetMonth(month);
			dt.SetDay(day);
			return dt;
		}

		/// <summary>Creates an <code>XMPDateTime</code>-object from initial values.</summary>
		/// <param name="year">years</param>
		/// <param name="month">
		/// months from 1 to 12<br />
		/// <em>Note:</em> Remember that the month in
		/// <see cref="Sharpen.Calendar"/>
		/// is defined from 0 to 11.
		/// </param>
		/// <param name="day">days</param>
		/// <param name="hour">hours</param>
		/// <param name="minute">minutes</param>
		/// <param name="second">seconds</param>
		/// <param name="nanoSecond">nanoseconds</param>
		/// <returns>Returns an <code>XMPDateTime</code>-object.</returns>
		public static XMPDateTime Create(int year, int month, int day, int hour, int minute, int second, int nanoSecond)
		{
			XMPDateTime dt = new XMPDateTimeImpl();
			dt.SetYear(year);
			dt.SetMonth(month);
			dt.SetDay(day);
			dt.SetHour(hour);
			dt.SetMinute(minute);
			dt.SetSecond(second);
			dt.SetNanoSecond(nanoSecond);
			return dt;
		}

		/// <summary>Creates an <code>XMPDateTime</code> from an ISO 8601 string.</summary>
		/// <param name="strValue">The ISO 8601 string representation of the date/time.</param>
		/// <returns>An <code>XMPDateTime</code>-object.</returns>
		/// <exception cref="XMPException">When the ISO 8601 string is non-conform</exception>
		/// <exception cref="Com.Adobe.Xmp.XMPException"/>
		public static XMPDateTime CreateFromISO8601(string strValue)
		{
			return new XMPDateTimeImpl(strValue);
		}

		/// <summary>Obtain the current date and time.</summary>
		/// <returns>
		/// Returns The returned time is UTC, properly adjusted for the local time zone. The
		/// resolution of the time is not guaranteed to be finer than seconds.
		/// </returns>
		public static XMPDateTime GetCurrentDateTime()
		{
			return new XMPDateTimeImpl(new Sharpen.GregorianCalendar());
		}

		/// <summary>
		/// Sets the local time zone without touching any other Any existing time zone value is replaced,
		/// the other date/time fields are not adjusted in any way.
		/// </summary>
		/// <param name="dateTime">the <code>XMPDateTime</code> variable containing the value to be modified.</param>
		/// <returns>Returns an updated <code>XMPDateTime</code>-object.</returns>
		public static XMPDateTime SetLocalTimeZone(XMPDateTime dateTime)
		{
			Sharpen.Calendar cal = dateTime.GetCalendar();
			cal.SetTimeZone(System.TimeZoneInfo.Local);
			return new XMPDateTimeImpl(cal);
		}

		/// <summary>Make sure a time is UTC.</summary>
		/// <remarks>
		/// Make sure a time is UTC. If the time zone is not UTC, the time is
		/// adjusted and the time zone set to be UTC.
		/// </remarks>
		/// <param name="dateTime">
		/// the <code>XMPDateTime</code> variable containing the time to
		/// be modified.
		/// </param>
		/// <returns>Returns an updated <code>XMPDateTime</code>-object.</returns>
		public static XMPDateTime ConvertToUTCTime(XMPDateTime dateTime)
		{
			long timeInMillis = dateTime.GetCalendar().GetTimeInMillis();
			Sharpen.GregorianCalendar cal = new Sharpen.GregorianCalendar(Utc);
			cal.SetGregorianChange(Sharpen.Extensions.CreateDate(long.MinValue));
			cal.SetTimeInMillis(timeInMillis);
			return new XMPDateTimeImpl(cal);
		}

		/// <summary>Make sure a time is local.</summary>
		/// <remarks>
		/// Make sure a time is local. If the time zone is not the local zone, the time is adjusted and
		/// the time zone set to be local.
		/// </remarks>
		/// <param name="dateTime">the <code>XMPDateTime</code> variable containing the time to be modified.</param>
		/// <returns>Returns an updated <code>XMPDateTime</code>-object.</returns>
		public static XMPDateTime ConvertToLocalTime(XMPDateTime dateTime)
		{
			long timeInMillis = dateTime.GetCalendar().GetTimeInMillis();
			// has automatically local timezone
			Sharpen.GregorianCalendar cal = new Sharpen.GregorianCalendar();
			cal.SetTimeInMillis(timeInMillis);
			return new XMPDateTimeImpl(cal);
		}
	}
}

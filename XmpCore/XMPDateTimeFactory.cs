// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;
using Sharpen;
using XmpCore.Impl;

namespace XmpCore
{
    /// <summary>
    /// A factory to create <c>XMPDateTime</c>-instances from a <c>Calendar</c> or an
    /// ISO 8601 string or for the current time.
    /// </summary>
    /// <since>16.02.2006</since>
    public static class XmpDateTimeFactory
    {
        /// <summary>Creates an <c>XMPDateTime</c> from a <c>Calendar</c>-object.</summary>
        /// <param name="calendar">a <c>Calendar</c>-object.</param>
        /// <returns>An <c>XMPDateTime</c>-object.</returns>
        public static IXmpDateTime CreateFromCalendar(Calendar calendar)
        {
            return new XmpDateTime(calendar);
        }

        /// <summary>Creates an empty <c>XMPDateTime</c>-object.</summary>
        /// <returns>Returns an <c>XMPDateTime</c>-object.</returns>
        public static IXmpDateTime Create()
        {
            return new XmpDateTime();
        }

        /// <summary>Creates an <c>XMPDateTime</c>-object from initial values.</summary>
        /// <param name="year">years</param>
        /// <param name="month">
        /// months from 1 to 12<br />
        /// <em>Note:</em> Remember that the month in
        /// <see cref="Sharpen.Calendar"/>
        /// is defined from 0 to 11.
        /// </param>
        /// <param name="day">days</param>
        /// <returns>Returns an <c>XMPDateTime</c>-object.</returns>
        public static IXmpDateTime Create(int year, int month, int day)
        {
            IXmpDateTime dt = new XmpDateTime();
            dt.SetYear(year);
            dt.SetMonth(month);
            dt.SetDay(day);
            return dt;
        }

        /// <summary>Creates an <c>XMPDateTime</c>-object from initial values.</summary>
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
        /// <returns>Returns an <c>XMPDateTime</c>-object.</returns>
        public static IXmpDateTime Create(int year, int month, int day, int hour, int minute, int second, int nanoSecond)
        {
            IXmpDateTime dt = new XmpDateTime();
            dt.SetYear(year);
            dt.SetMonth(month);
            dt.SetDay(day);
            dt.SetHour(hour);
            dt.SetMinute(minute);
            dt.SetSecond(second);
            dt.SetNanoSecond(nanoSecond);
            return dt;
        }

        /// <summary>Creates an <c>XMPDateTime</c> from an ISO 8601 string.</summary>
        /// <param name="strValue">The ISO 8601 string representation of the date/time.</param>
        /// <returns>An <c>XMPDateTime</c>-object.</returns>
        /// <exception cref="XmpException">When the ISO 8601 string is non-conform</exception>
        /// <exception cref="XmpException"/>
        public static IXmpDateTime CreateFromIso8601(string strValue)
        {
            return new XmpDateTime(strValue);
        }

        /// <summary>Obtain the current date and time.</summary>
        /// <returns>
        /// Returns The returned time is UTC, properly adjusted for the local time zone. The
        /// resolution of the time is not guaranteed to be finer than seconds.
        /// </returns>
        public static IXmpDateTime GetCurrentDateTime()
        {
            return new XmpDateTime(new GregorianCalendar());
        }

        /// <summary>
        /// Sets the local time zone without touching any other Any existing time zone value is replaced,
        /// the other date/time fields are not adjusted in any way.
        /// </summary>
        /// <param name="dateTime">the <c>XMPDateTime</c> variable containing the value to be modified.</param>
        /// <returns>Returns an updated <c>XMPDateTime</c>-object.</returns>
        public static IXmpDateTime SetLocalTimeZone(IXmpDateTime dateTime)
        {
            var cal = dateTime.GetCalendar();
            cal.SetTimeZone(TimeZoneInfo.Local);
            return new XmpDateTime(cal);
        }

        /// <summary>Make sure a time is UTC.</summary>
        /// <remarks>
        /// Make sure a time is UTC. If the time zone is not UTC, the time is
        /// adjusted and the time zone set to be UTC.
        /// </remarks>
        /// <param name="dateTime">
        /// the <c>XMPDateTime</c> variable containing the time to
        /// be modified.
        /// </param>
        /// <returns>Returns an updated <c>XMPDateTime</c>-object.</returns>
        public static IXmpDateTime ConvertToUtcTime(IXmpDateTime dateTime)
        {
            var timeInMillis = dateTime.GetCalendar().GetTimeInMillis();
            var cal = new GregorianCalendar(TimeZoneInfo.Utc);
            cal.SetGregorianChange(XmpDateTime.UnixTimeToDateTime(long.MinValue));
            cal.SetTimeInMillis(timeInMillis);
            return new XmpDateTime(cal);
        }

        /// <summary>Make sure a time is local.</summary>
        /// <remarks>
        /// Make sure a time is local. If the time zone is not the local zone, the time is adjusted and
        /// the time zone set to be local.
        /// </remarks>
        /// <param name="dateTime">the <c>XMPDateTime</c> variable containing the time to be modified.</param>
        /// <returns>Returns an updated <c>XMPDateTime</c>-object.</returns>
        public static IXmpDateTime ConvertToLocalTime(IXmpDateTime dateTime)
        {
            var timeInMillis = dateTime.GetCalendar().GetTimeInMillis();
            // has automatically local timezone
            var cal = new GregorianCalendar();
            cal.SetTimeInMillis(timeInMillis);
            return new XmpDateTime(cal);
        }
    }
}

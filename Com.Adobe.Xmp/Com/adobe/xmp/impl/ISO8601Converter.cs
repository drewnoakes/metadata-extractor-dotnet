// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;
using System.Text;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>Converts between ISO 8601 Strings and <c>Calendar</c> with millisecond resolution.</summary>
    /// <since>16.02.2006</since>
    public static class Iso8601Converter
    {
        // EMPTY
        /// <summary>Converts an ISO 8601 string to an <c>XMPDateTime</c>.</summary>
        /// <remarks>
        /// Converts an ISO 8601 string to an <c>XMPDateTime</c>.
        /// Parse a date according to ISO 8601 and
        /// http://www.w3.org/TR/NOTE-datetime:
        /// <list type="bullet">
        /// <item>YYYY
        /// <item>YYYY-MM
        /// <item>YYYY-MM-DD
        /// <item>YYYY-MM-DDThh:mmTZD
        /// <item>YYYY-MM-DDThh:mm:ssTZD
        /// <item>YYYY-MM-DDThh:mm:ss.sTZD
        /// </list>
        /// Data fields:
        /// <list type="bullet">
        /// <item>YYYY = four-digit year
        /// <item>MM = two-digit month (01=January, etc.)
        /// <item>DD = two-digit day of month (01 through 31)
        /// <item>hh = two digits of hour (00 through 23)
        /// <item>mm = two digits of minute (00 through 59)
        /// <item>ss = two digits of second (00 through 59)
        /// <item>s = one or more digits representing a decimal fraction of a second
        /// <item>TZD = time zone designator (Z or +hh:mm or -hh:mm)
        /// </list>
        /// Note that ISO 8601 does not seem to allow years less than 1000 or greater
        /// than 9999. We allow any year, even negative ones. The year is formatted
        /// as "%.4d".
        /// <para>
        /// <em>Note:</em> Tolerate missing TZD, assume is UTC. Photoshop 8 writes
        /// dates like this for exif:GPSTimeStamp.<br />
        /// <em>Note:</em> DOES NOT APPLY ANYMORE.
        /// Tolerate missing date portion, in case someone foolishly
        /// writes a time-only value that way.
        /// </remarks>
        /// <param name="iso8601String">a date string that is ISO 8601 conform.</param>
        /// <returns>Returns a <c>Calendar</c>.</returns>
        /// <exception cref="XmpException">Is thrown when the string is non-conform.</exception>
        public static IXmpDateTime Parse(string iso8601String)
        {
            return Parse(iso8601String, new XmpDateTime());
        }

        /// <param name="iso8601String">a date string that is ISO 8601 conform.</param>
        /// <param name="binValue">an existing XMPDateTime to set with the parsed date</param>
        /// <returns>Returns an XMPDateTime-object containing the ISO8601-date.</returns>
        /// <exception cref="XmpException">Is thrown when the string is non-conform.</exception>
        public static IXmpDateTime Parse(string iso8601String, IXmpDateTime binValue)
        {
            if (iso8601String == null)
            {
                throw new XmpException("Parameter must not be null", XmpErrorCode.Badparam);
            }
            if (iso8601String.Length == 0)
            {
                return binValue;
            }
            ParseState input = new ParseState(iso8601String);
            int value;
            if (input.Ch(0) == '-')
            {
                input.Skip();
            }
            // Extract the year.
            value = input.GatherInt("Invalid year in date string", 9999);
            if (input.HasNext() && input.Ch() != '-')
            {
                throw new XmpException("Invalid date string, after year", XmpErrorCode.Badvalue);
            }
            if (input.Ch(0) == '-')
            {
                value = -value;
            }
            binValue.SetYear(value);
            if (!input.HasNext())
            {
                return binValue;
            }
            input.Skip();
            // Extract the month.
            value = input.GatherInt("Invalid month in date string", 12);
            if (input.HasNext() && input.Ch() != '-')
            {
                throw new XmpException("Invalid date string, after month", XmpErrorCode.Badvalue);
            }
            binValue.SetMonth(value);
            if (!input.HasNext())
            {
                return binValue;
            }
            input.Skip();
            // Extract the day.
            value = input.GatherInt("Invalid day in date string", 31);
            if (input.HasNext() && input.Ch() != 'T')
            {
                throw new XmpException("Invalid date string, after day", XmpErrorCode.Badvalue);
            }
            binValue.SetDay(value);
            if (!input.HasNext())
            {
                return binValue;
            }
            input.Skip();
            // Extract the hour.
            value = input.GatherInt("Invalid hour in date string", 23);
            binValue.SetHour(value);
            if (!input.HasNext())
            {
                return binValue;
            }
            // Extract the minute.
            if (input.Ch() == ':')
            {
                input.Skip();
                value = input.GatherInt("Invalid minute in date string", 59);
                if (input.HasNext() && input.Ch() != ':' && input.Ch() != 'Z' && input.Ch() != '+' && input.Ch() != '-')
                {
                    throw new XmpException("Invalid date string, after minute", XmpErrorCode.Badvalue);
                }
                binValue.SetMinute(value);
            }
            if (!input.HasNext())
            {
                return binValue;
            }
            if (input.HasNext() && input.Ch() == ':')
            {
                input.Skip();
                value = input.GatherInt("Invalid whole seconds in date string", 59);
                if (input.HasNext() && input.Ch() != '.' && input.Ch() != 'Z' && input.Ch() != '+' && input.Ch() != '-')
                {
                    throw new XmpException("Invalid date string, after whole seconds", XmpErrorCode.Badvalue);
                }
                binValue.SetSecond(value);
                if (input.Ch() == '.')
                {
                    input.Skip();
                    int digits = input.Pos();
                    value = input.GatherInt("Invalid fractional seconds in date string", 999999999);
                    if (input.HasNext() && (input.Ch() != 'Z' && input.Ch() != '+' && input.Ch() != '-'))
                    {
                        throw new XmpException("Invalid date string, after fractional second", XmpErrorCode.Badvalue);
                    }
                    digits = input.Pos() - digits;
                    for (; digits > 9; --digits)
                    {
                        value = value / 10;
                    }
                    for (; digits < 9; ++digits)
                    {
                        value = value * 10;
                    }
                    binValue.SetNanoSecond(value);
                }
            }
            else
            {
                if (input.Ch() != 'Z' && input.Ch() != '+' && input.Ch() != '-')
                {
                    throw new XmpException("Invalid date string, after time", XmpErrorCode.Badvalue);
                }
            }
            int tzSign = 0;
            int tzHour = 0;
            int tzMinute = 0;
            if (!input.HasNext())
            {
                // no Timezone at all
                return binValue;
            }
            if (input.Ch() == 'Z')
            {
                input.Skip();
            }
            else
            {
                if (input.HasNext())
                {
                    if (input.Ch() == '+')
                    {
                        tzSign = 1;
                    }
                    else
                    {
                        if (input.Ch() == '-')
                        {
                            tzSign = -1;
                        }
                        else
                        {
                            throw new XmpException("Time zone must begin with 'Z', '+', or '-'", XmpErrorCode.Badvalue);
                        }
                    }
                    input.Skip();
                    // Extract the time zone hour.
                    tzHour = input.GatherInt("Invalid time zone hour in date string", 23);
                    if (input.HasNext())
                    {
                        if (input.Ch() == ':')
                        {
                            input.Skip();
                            // Extract the time zone minute.
                            tzMinute = input.GatherInt("Invalid time zone minute in date string", 59);
                        }
                        else
                        {
                            throw new XmpException("Invalid date string, after time zone hour", XmpErrorCode.Badvalue);
                        }
                    }
                }
            }
            // create a corresponding TZ and set it time zone
            int offset = (tzHour * 3600 * 1000 + tzMinute * 60 * 1000) * tzSign;
            binValue.SetTimeZone((TimeZoneInfo)new SimpleTimeZone(offset, string.Empty));
            if (input.HasNext())
            {
                throw new XmpException("Invalid date string, extra chars at end", XmpErrorCode.Badvalue);
            }
            return binValue;
        }

        /// <summary>Converts a <c>Calendar</c> into an ISO 8601 string.</summary>
        /// <remarks>
        /// Converts a <c>Calendar</c> into an ISO 8601 string.
        /// Format a date according to ISO 8601 and http://www.w3.org/TR/NOTE-datetime:
        /// <list type="bullet">
        /// <item>YYYY
        /// <item>YYYY-MM
        /// <item>YYYY-MM-DD
        /// <item>YYYY-MM-DDThh:mmTZD
        /// <item>YYYY-MM-DDThh:mm:ssTZD
        /// <item>YYYY-MM-DDThh:mm:ss.sTZD
        /// </list>
        /// Data fields:
        /// <list type="bullet">
        /// <item>YYYY = four-digit year
        /// <item>MM     = two-digit month (01=January, etc.)
        /// <item>DD     = two-digit day of month (01 through 31)
        /// <item>hh     = two digits of hour (00 through 23)
        /// <item>mm     = two digits of minute (00 through 59)
        /// <item>ss     = two digits of second (00 through 59)
        /// <item>s     = one or more digits representing a decimal fraction of a second
        /// <item>TZD     = time zone designator (Z or +hh:mm or -hh:mm)
        /// </list>
        /// <para>
        /// <em>Note:</em> ISO 8601 does not seem to allow years less than 1000 or greater than 9999.
        /// We allow any year, even negative ones. The year is formatted as "%.4d".<para>
        /// <em>Note:</em> Fix for bug 1269463 (silently fix out of range values) included in parsing.
        /// The quasi-bogus "time only" values from Photoshop CS are not supported.
        /// </remarks>
        /// <param name="dateTime">an XMPDateTime-object.</param>
        /// <returns>Returns an ISO 8601 string.</returns>
        public static string Render(IXmpDateTime dateTime)
        {
            StringBuilder buffer = new StringBuilder();
            if (dateTime.HasDate())
            {
                // year is rendered in any case, even 0000
                DecimalFormat df = new DecimalFormat("0000", new DecimalFormatSymbols(Extensions.GetEnglishCulture()));
                buffer.Append(df.Format(dateTime.GetYear()));
                if (dateTime.GetMonth() == 0)
                {
                    return buffer.ToString();
                }
                // month
                df.ApplyPattern("'-'00");
                buffer.Append(df.Format(dateTime.GetMonth()));
                if (dateTime.GetDay() == 0)
                {
                    return buffer.ToString();
                }
                // day
                buffer.Append(df.Format(dateTime.GetDay()));
                // time, rendered if any time field is not zero
                if (dateTime.HasTime())
                {
                    // hours and minutes
                    buffer.Append('T');
                    df.ApplyPattern("00");
                    buffer.Append(df.Format(dateTime.GetHour()));
                    buffer.Append(':');
                    buffer.Append(df.Format(dateTime.GetMinute()));
                    // seconds and nanoseconds
                    if (dateTime.GetSecond() != 0 || dateTime.GetNanoSecond() != 0)
                    {
                        double seconds = dateTime.GetSecond() + dateTime.GetNanoSecond() / 1e9d;
                        df.ApplyPattern(":00.#########");
                        buffer.Append(df.Format(seconds));
                    }
                    // time zone
                    if (dateTime.HasTimeZone())
                    {
                        // used to calculate the time zone offset incl. Daylight Savings
                        long timeInMillis = dateTime.GetCalendar().GetTimeInMillis();
                        int offset = dateTime.GetTimeZone().GetOffset(timeInMillis);
                        if (offset == 0)
                        {
                            // UTC
                            buffer.Append('Z');
                        }
                        else
                        {
                            int thours = offset / 3600000;
                            int tminutes = Math.Abs(offset % 3600000 / 60000);
                            df.ApplyPattern("+00;-00");
                            buffer.Append(df.Format(thours));
                            df.ApplyPattern(":00");
                            buffer.Append(df.Format(tminutes));
                        }
                    }
                }
            }
            return buffer.ToString();
        }
    }

    /// <since>22.08.2006</since>
    internal class ParseState
    {
        private readonly string _str;

        private int _pos;

        /// <param name="str">initializes the parser container</param>
        public ParseState(string str)
        {
            _str = str;
        }

        /// <returns>Returns the length of the input.</returns>
        public virtual int Length()
        {
            return _str.Length;
        }

        /// <returns>Returns whether there are more chars to come.</returns>
        public virtual bool HasNext()
        {
            return _pos < _str.Length;
        }

        /// <param name="index">index of char</param>
        /// <returns>Returns char at a certain index.</returns>
        public virtual char Ch(int index)
        {
            return index < _str.Length ? _str[index] : (char)0x0000;
        }

        /// <returns>Returns the current char or 0x0000 if there are no more chars.</returns>
        public virtual char Ch()
        {
            return _pos < _str.Length ? _str[_pos] : (char)0x0000;
        }

        /// <summary>Skips the next char.</summary>
        public virtual void Skip()
        {
            _pos++;
        }

        /// <returns>Returns the current position.</returns>
        public virtual int Pos()
        {
            return _pos;
        }

        /// <summary>Parses a integer from the source and sets the pointer after it.</summary>
        /// <param name="errorMsg">Error message to put in the exception if no number can be found</param>
        /// <param name="maxValue">the max value of the number to return</param>
        /// <returns>Returns the parsed integer.</returns>
        /// <exception cref="XmpException">Thrown if no integer can be found.</exception>
        public virtual int GatherInt(string errorMsg, int maxValue)
        {
            int value = 0;
            bool success = false;
            char ch = Ch(_pos);
            while ('0' <= ch && ch <= '9')
            {
                value = (value * 10) + (ch - '0');
                success = true;
                _pos++;
                ch = Ch(_pos);
            }
            if (success)
            {
                if (value > maxValue)
                {
                    return maxValue;
                }
                if (value < 0)
                {
                    return 0;
                }
                return value;
            }
            throw new XmpException(errorMsg, XmpErrorCode.Badvalue);
        }
    }
}

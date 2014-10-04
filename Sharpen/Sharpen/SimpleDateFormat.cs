using System.Text.RegularExpressions;

namespace Sharpen
{
	using System;
	using System.Globalization;

	public class SimpleDateFormat : DateFormat
	{
        private const string FIELD_YEAR = "year";
        private const string FIELD_MONTH = "month";
        private const string FIELD_DAY = "day";
        private const string FIELD_HOUR = "hour";
        private const string FIELD_MINUTE = "minute";
        private const string FIELD_SECOND = "second";

		string format;

	    CultureInfo Culture {
			get; set;
		}
		
		public SimpleDateFormat (): this ("g")
		{
		}

		public SimpleDateFormat (string format): this (format, CultureInfo.CurrentCulture)
		{
		}

		public SimpleDateFormat (string format, CultureInfo c)
		{
			Culture = c;
			this.format = format.Replace ("EEE", "ddd");
			this.format = this.format.Replace ("Z", "zzz");
			SetTimeZone (TimeZoneInfo.Local);
		}

		public override DateTime Parse (string value)
		{
		    DateTime? result;
		    if (TryDateTimeParse(value, out result))
		    {
		        return result.Value;
		    }

		    try
		    {
		        return DateTime.ParseExact(value, format, Culture);
		    }
		    catch (Exception ex)
		    {
		        throw new ParseException();
		    }
		}

	    public override string Format (DateTime date)
		{
			date += GetTimeZone().BaseUtcOffset;
			return date.ToString (format);
		}
		
		public string Format (long date)
		{
			return Extensions.MillisToDateTimeOffset (date, (int)GetTimeZone ().BaseUtcOffset.TotalMinutes).DateTime.ToString (format);
		}

        private bool TryDateTimeParse(string value, out DateTime? result)
        {
            var formatParts = GetParsedFormat();
            if (formatParts.Length == 0)
            {
                result = null;
                return false;
            }
            var parser = BuildDateTimeParser(formatParts);
            try
            {
                var parseResult = parser.Matches(value);
                if (parseResult.Count == 0)
                {
                    result = null;
                    return false;
                }
                result = ConvertResult(parseResult[0]);
                return true;
            }
            catch
            {
            }

            result = null;
            return false;
        }

	    private string[] GetParsedFormat()
	    {
            var r = new Regex("(yyyy|MM|dd|HH|mm|ss)");
            return r.Split(format);
	    }

        private Regex BuildDateTimeParser(string[] formatParts)
        {
            string pattern = "";

            foreach (var formatPart in formatParts)
            {
                if (string.IsNullOrEmpty(formatPart))
                {
                    continue;
                }

                if (formatPart.Equals("yyyy"))
                {
                    pattern += string.Format("(?<{0}>\\d{{4}})", FIELD_YEAR);
                    continue;
                }
                
                if (formatPart.Equals("MM"))
                {
                    pattern += string.Format("(?<{0}>\\d{{2}})", FIELD_MONTH);
                    continue;
                }
                
                if (formatPart.Equals("dd"))
                {
                    pattern += string.Format("(?<{0}>\\d{{2}})", FIELD_DAY);
                    continue;
                }
                
                if (formatPart.Equals("HH"))
                {
                    pattern += string.Format("(?<{0}>\\d{{2}})", FIELD_HOUR);
                    continue;
                }
                
                if (formatPart.Equals("mm"))
                {
                    pattern += string.Format("(?<{0}>\\d{{2}})", FIELD_MINUTE);
                    continue;
                }
                
                if (formatPart.Equals("ss"))
                {
                    pattern += string.Format("(?<{0}>\\d{{2}})", FIELD_SECOND);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(formatPart))
                {
                    pattern += string.Format("\\s{{{0}}}", formatPart.Length);
                    continue;
                }

                pattern += formatPart;
            }

            return new Regex(pattern);
        }

        private DateTime ConvertResult(Match data)
        {
            Calendar result = Calendar.GetInstance(Culture);
            if (data.Groups[FIELD_YEAR].Success)
            {
                result.Set(CalendarEnum.Year, int.Parse(data.Groups[FIELD_YEAR].Value));
            }
            else
            {
                result.Set(CalendarEnum.Year, 1);
            }
            
            if (data.Groups[FIELD_MONTH].Success)
            {
                result.Set(CalendarEnum.MonthOneBased, int.Parse(data.Groups[FIELD_MONTH].Value));
            }
            else
            {
                result.Set(CalendarEnum.MonthOneBased, 1);
            }
            
            if (data.Groups[FIELD_DAY].Success)
            {
                result.Set(CalendarEnum.DayOfMonth, int.Parse(data.Groups[FIELD_DAY].Value));
            }
            else
            {
                result.Set(CalendarEnum.DayOfMonth, 0);
            }
            
            if (data.Groups[FIELD_HOUR].Success)
            {
                result.Set(CalendarEnum.HourOfDay, int.Parse(data.Groups[FIELD_HOUR].Value));
            }
            else
            {
                result.Set(CalendarEnum.HourOfDay, 0);
            }
            
            if (data.Groups[FIELD_MINUTE].Success)
            {
                result.Set(CalendarEnum.Minute, int.Parse(data.Groups[FIELD_MINUTE].Value));
            }
            else
            {
                result.Set(CalendarEnum.Minute, 0);
            }
            
            if (data.Groups[FIELD_SECOND].Success)
            {
                result.Set(CalendarEnum.Second, int.Parse(data.Groups[FIELD_SECOND].Value));
            }
            else
            {
                result.Set(CalendarEnum.Second, 0);
            }

            result.Set(CalendarEnum.Millisecond, 0);

            return result.GetTime();
        }
	}
}

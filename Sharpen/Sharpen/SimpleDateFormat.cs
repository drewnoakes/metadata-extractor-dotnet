using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sharpen
{
    public class SimpleDateFormat : DateFormat
    {
        private const string FieldYear = "year";
        private const string FieldMonth = "month";
        private const string FieldDay = "day";
        private const string FieldHour = "hour";
        private const string FieldMinute = "minute";
        private const string FieldSecond = "second";

        readonly string _format;

        CultureInfo Culture { get; set; }

        public SimpleDateFormat (string format): this (format, CultureInfo.CurrentCulture)
        {
        }

        public SimpleDateFormat (string format, CultureInfo c)
        {
            Culture = c;
            this._format = format.Replace ("EEE", "ddd");
            this._format = this._format.Replace ("Z", "zzz");
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
                return DateTime.ParseExact(value, _format, Culture);
            }
            catch (Exception ex)
            {
                throw new ParseException();
            }
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
            return r.Split(_format);
        }

        private static Regex BuildDateTimeParser(string[] formatParts)
        {
            var pattern = new StringBuilder();

            foreach (var formatPart in formatParts)
            {
                if (string.IsNullOrEmpty(formatPart))
                {
                    continue;
                }

                if (formatPart.Equals("yyyy"))
                {
                    pattern.AppendFormat("(?<{0}>\\d{{4}})", FieldYear);
                    continue;
                }

                if (formatPart.Equals("MM"))
                {
                    pattern.AppendFormat("(?<{0}>\\d{{2}})", FieldMonth);
                    continue;
                }

                if (formatPart.Equals("dd"))
                {
                    pattern.AppendFormat("(?<{0}>\\d{{2}})", FieldDay);
                    continue;
                }

                if (formatPart.Equals("HH"))
                {
                    pattern.AppendFormat("(?<{0}>\\d{{2}})", FieldHour);
                    continue;
                }

                if (formatPart.Equals("mm"))
                {
                    pattern.AppendFormat("(?<{0}>\\d{{2}})", FieldMinute);
                    continue;
                }

                if (formatPart.Equals("ss"))
                {
                    pattern.AppendFormat("(?<{0}>\\d{{2}})", FieldSecond);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(formatPart))
                {
                    pattern.AppendFormat("\\s{{{0}}}", formatPart.Length);
                    continue;
                }

                pattern.Append(formatPart);
            }

            return new Regex(pattern.ToString());
        }

        private DateTime ConvertResult(Match data)
        {
            Calendar result = Calendar.GetInstance(Culture);
            if (data.Groups[FieldYear].Success)
            {
                result.Set(CalendarEnum.Year, int.Parse(data.Groups[FieldYear].Value));
            }
            else
            {
                result.Set(CalendarEnum.Year, 1);
            }

            if (data.Groups[FieldMonth].Success)
            {
                result.Set(CalendarEnum.MonthOneBased, int.Parse(data.Groups[FieldMonth].Value));
            }
            else
            {
                result.Set(CalendarEnum.MonthOneBased, 1);
            }

            if (data.Groups[FieldDay].Success)
            {
                result.Set(CalendarEnum.DayOfMonth, int.Parse(data.Groups[FieldDay].Value));
            }
            else
            {
                result.Set(CalendarEnum.DayOfMonth, 0);
            }

            if (data.Groups[FieldHour].Success)
            {
                result.Set(CalendarEnum.HourOfDay, int.Parse(data.Groups[FieldHour].Value));
            }
            else
            {
                result.Set(CalendarEnum.HourOfDay, 0);
            }

            if (data.Groups[FieldMinute].Success)
            {
                result.Set(CalendarEnum.Minute, int.Parse(data.Groups[FieldMinute].Value));
            }
            else
            {
                result.Set(CalendarEnum.Minute, 0);
            }

            if (data.Groups[FieldSecond].Success)
            {
                result.Set(CalendarEnum.Second, int.Parse(data.Groups[FieldSecond].Value));
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

/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using System.Text;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata
{
    /// <summary>Represents an age in years, months, days, hours, minutes and seconds.</summary>
    /// <remarks>
    /// Represents an age in years, months, days, hours, minutes and seconds.
    /// <para>
    /// Used by certain Panasonic cameras which have face recognition features.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class Age
    {
        private readonly int _years;

        private readonly int _months;

        private readonly int _days;

        private readonly int _hours;

        private readonly int _minutes;

        private readonly int _seconds;

        /// <summary>
        /// Parses an age object from the string format used by Panasonic cameras:
        /// <c>0031:07:15 00:00:00</c>
        /// </summary>
        /// <param name="s">The String in format <c>0031:07:15 00:00:00</c>.</param>
        /// <returns>The parsed Age object, or null if the value could not be parsed</returns>
        [CanBeNull]
        public static Age FromPanasonicString([NotNull] string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException();
            }
            if (s.Length != 19 || s.StartsWith("9999:99:99"))
            {
                return null;
            }
            try
            {
                int years = Convert.ToInt32(s.Substring (0, 4 - 0));
                int months = Convert.ToInt32(s.Substring (5, 7 - 5));
                int days = Convert.ToInt32(s.Substring (8, 10 - 8));
                int hours = Convert.ToInt32(s.Substring (11, 13 - 11));
                int minutes = Convert.ToInt32(s.Substring (14, 16 - 14));
                int seconds = Convert.ToInt32(s.Substring (17, 19 - 17));
                return new Age(years, months, days, hours, minutes, seconds);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public Age(int years, int months, int days, int hours, int minutes, int seconds)
        {
            _years = years;
            _months = months;
            _days = days;
            _hours = hours;
            _minutes = minutes;
            _seconds = seconds;
        }

        public int GetYears()
        {
            return _years;
        }

        public int GetMonths()
        {
            return _months;
        }

        public int GetDays()
        {
            return _days;
        }

        public int GetHours()
        {
            return _hours;
        }

        public int GetMinutes()
        {
            return _minutes;
        }

        public int GetSeconds()
        {
            return _seconds;
        }

        public override string ToString()
        {
            return string.Format("{0:D4}:{1:D2}:{2:D2} {3:D2}:{4:D2}:{5:D2}", _years, _months, _days, _hours, _minutes, _seconds);
        }

        public string ToFriendlyString()
        {
            StringBuilder result = new StringBuilder();
            AppendAgePart(result, _years, "year");
            AppendAgePart(result, _months, "month");
            AppendAgePart(result, _days, "day");
            AppendAgePart(result, _hours, "hour");
            AppendAgePart(result, _minutes, "minute");
            AppendAgePart(result, _seconds, "second");
            return Extensions.ConvertToString(result);
        }

        private static void AppendAgePart(StringBuilder result, int num, string singularName)
        {
            if (num == 0)
            {
                return;
            }
            if (result.Length != 0)
            {
                result.Append(' ');
            }
            result.Append(num).Append(' ').Append(singularName);
            if (num != 1)
            {
                result.Append('s');
            }
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            Age age = (Age)o;
            if (_days != age._days)
            {
                return false;
            }
            if (_hours != age._hours)
            {
                return false;
            }
            if (_minutes != age._minutes)
            {
                return false;
            }
            if (_months != age._months)
            {
                return false;
            }
            if (_seconds != age._seconds)
            {
                return false;
            }
            if (_years != age._years)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int result = _years;
            result = 31 * result + _months;
            result = 31 * result + _days;
            result = 31 * result + _hours;
            result = 31 * result + _minutes;
            result = 31 * result + _seconds;
            return result;
        }
    }
}

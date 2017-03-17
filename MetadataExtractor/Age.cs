#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using System.Text;
using JetBrains.Annotations;

namespace MetadataExtractor
{
    /// <summary>Represents an age in years, months, days, hours, minutes and seconds.</summary>
    /// <remarks>
    /// Used by certain Panasonic cameras which have face recognition features.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class Age
    {
        /// <summary>
        /// Parses an age object from the string format used by Panasonic cameras:
        /// <c>0031:07:15 00:00:00</c>
        /// </summary>
        /// <param name="s">The string in format <c>0031:07:15 00:00:00</c>.</param>
        /// <returns>The parsed Age object, or null if the value could not be parsed</returns>
        [CanBeNull]
        public static Age FromPanasonicString([NotNull] string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            if (s.Length != 19 || s.StartsWith("9999:99:99"))
                return null;

            try
            {
                return new Age(
                    years: int.Parse(s.Substring (0, 4 - 0)),
                    months: int.Parse(s.Substring (5, 7 - 5)),
                    days: int.Parse(s.Substring (8, 10 - 8)),
                    hours: int.Parse(s.Substring (11, 13 - 11)),
                    minutes: int.Parse(s.Substring (14, 16 - 14)),
                    seconds: int.Parse(s.Substring (17, 19 - 17)));
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public Age(int years, int months, int days, int hours, int minutes, int seconds)
        {
            Years = years;
            Months = months;
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }

        public int Years { get; }
        public int Months { get; }
        public int Days { get; }
        public int Hours { get; }
        public int Minutes { get; }
        public int Seconds { get; }

        public override string ToString()
        {
            return $"{Years:D4}:{Months:D2}:{Days:D2} {Hours:D2}:{Minutes:D2}:{Seconds:D2}";
        }

        [NotNull]
        public string ToFriendlyString()
        {
            var result = new StringBuilder();
            AppendAgePart(result, Years, "year");
            AppendAgePart(result, Months, "month");
            AppendAgePart(result, Days, "day");
            AppendAgePart(result, Hours, "hour");
            AppendAgePart(result, Minutes, "minute");
            AppendAgePart(result, Seconds, "second");
            return result.ToString();
        }

        private static void AppendAgePart([NotNull] StringBuilder result, int num, string singularName)
        {
            if (num == 0)
                return;
            if (result.Length != 0)
                result.Append(' ');
            result.Append(num).Append(' ').Append(singularName);
            if (num != 1)
                result.Append('s');
        }

        #region Equality and hashing

        private bool Equals([NotNull] Age other)
        {
            return Years == other.Years && Months == other.Months && Days == other.Days && Hours == other.Hours && Minutes == other.Minutes && Seconds == other.Seconds;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is Age age && Equals(age);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Years;
                hashCode = (hashCode*397) ^ Months;
                hashCode = (hashCode*397) ^ Days;
                hashCode = (hashCode*397) ^ Hours;
                hashCode = (hashCode*397) ^ Minutes;
                hashCode = (hashCode*397) ^ Seconds;
                return hashCode;
            }
        }

        #endregion
    }
}

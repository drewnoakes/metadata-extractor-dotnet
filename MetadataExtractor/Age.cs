// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor
{
    /// <summary>Represents an age in years, months, days, hours, minutes and seconds.</summary>
    /// <remarks>
    /// Used by certain Panasonic cameras which have face recognition features.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class Age(int years, int months, int days, int hours, int minutes, int seconds)
    {
        public int Years { get; } = years;

        public int Months { get; } = months;

        public int Days { get; } = days;

        public int Hours { get; } = hours;

        public int Minutes { get; } = minutes;

        public int Seconds { get; } = seconds;

        /// <summary>
        /// Parses an age object from the string format used by Panasonic cameras:
        /// <c>0031:07:15 00:00:00</c>
        /// </summary>
        /// <param name="s">The string in format <c>0031:07:15 00:00:00</c>.</param>
        /// <returns>The parsed Age object, or <see langword="null"/> if the value could not be parsed</returns>
        /// <exception cref="ArgumentNullException">The string <paramref name="s"/> is <see langword="null"/>.</exception>
        public static Age? FromPanasonicString(string s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));

            if (s.Length != 19 || s.StartsWith("9999:99:99", StringComparison.Ordinal))
                return null;

            if (int.TryParse(s.Substring(0, 4), out var years) &&
                int.TryParse(s.Substring(5, 2), out var months) &&
                int.TryParse(s.Substring(8, 2), out var days) &&
                int.TryParse(s.Substring(11, 2), out var hours) &&
                int.TryParse(s.Substring(14, 2), out var minutes) &&
                int.TryParse(s.Substring(17, 2), out var seconds))
            {
                return new Age(years, months, days, hours, minutes, seconds);
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Years:D4}:{Months:D2}:{Days:D2} {Hours:D2}:{Minutes:D2}:{Seconds:D2}";
        }

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

        private static void AppendAgePart(StringBuilder result, int num, string singularName)
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

        private bool Equals(Age? other)
        {
            return other is not null && Years == other.Years && Months == other.Months && Days == other.Days && Hours == other.Hours && Minutes == other.Minutes && Seconds == other.Seconds;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is Age age && Equals(age);
        }

        public override int GetHashCode()
        {
#if NET8_0_OR_GREATER
            HashCode hash = new();
            hash.Add(Years);
            hash.Add(Months);
            hash.Add(Days);
            hash.Add(Hours);
            hash.Add(Minutes);
            hash.Add(Seconds);
            return hash.ToHashCode();
#else
            unchecked
            {
                var hashCode = Years;
                hashCode = (hashCode * 397) ^ Months;
                hashCode = (hashCode * 397) ^ Days;
                hashCode = (hashCode * 397) ^ Hours;
                hashCode = (hashCode * 397) ^ Minutes;
                hashCode = (hashCode * 397) ^ Seconds;
                return hashCode;
            }
#endif
        }

        #endregion
    }
}

// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;

namespace MetadataExtractor.Util
{
    /// <summary>
    /// Utility methods for date and time values.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    internal static class DateUtil
    {
        [Pure]
        public static bool IsValidDate(int year, int month, int day)
            => year >= 1 && year <= 9999 &&
               month >= 1 && month <= 12 &&
               day >= 1 && day <= DateTime.DaysInMonth(year, month);

        [Pure]
        public static bool IsValidTime(int hours, int minutes, int seconds)
            => hours >= 0 && hours < 24 &&
               minutes >= 0 && minutes < 60 &&
               seconds >= 0 && seconds < 60;

        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTime(long unixTime) => _unixEpoch.AddSeconds(unixTime);
    }
}

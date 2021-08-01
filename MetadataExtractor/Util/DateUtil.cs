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
            => year is >= 1 and <= 9999 &&
               month is >= 1 and <= 12 &&
               day >= 1 && day <= DateTime.DaysInMonth(year, month);

        [Pure]
        public static bool IsValidTime(int hours, int minutes, int seconds)
            => hours is >= 0 and < 24 &&
               minutes is >= 0 and < 60
               && seconds is >= 0 and < 60;

        private static readonly DateTime _unixEpoch = new(1970, 1, 1, 0, 0, 0);

        public static DateTime FromUnixTime(long unixTime) => _unixEpoch.AddSeconds(unixTime);
    }
}

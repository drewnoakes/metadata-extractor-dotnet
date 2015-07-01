#region License
//
// Copyright 2002-2015 Drew Noakes
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
        {
            return year >= 1 && year <= 9999 &&
                   month >= 1 && month <= 12 &&
                   day >= 1 && day <= DateTime.DaysInMonth(year, month);
        }

        [Pure]
        public static bool IsValidTime(int hours, int minutes, int seconds)
        {
            return hours >= 0 && hours < 24 &&
                   minutes >= 0 && minutes < 60 &&
                   seconds >= 0 && seconds < 60;
        }
    }
}
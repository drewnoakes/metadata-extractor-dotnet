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
using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;
using Sharpen;

namespace MetadataExtractor
{
    /// <summary>Represents a latitude and longitude pair, giving a position on earth in spherical coordinates.</summary>
    /// <remarks>
    /// Represents a latitude and longitude pair, giving a position on earth in spherical coordinates.
    /// <para />
    /// Values of latitude and longitude are given in degrees.
    /// <para />
    /// This type is immutable.
    /// </remarks>
    public sealed class GeoLocation
    {
        private readonly double _latitude;

        private readonly double _longitude;

        /// <summary>
        /// Instantiates a new instance of <see cref="GeoLocation"/>.
        /// </summary>
        /// <param name="latitude">the latitude, in degrees</param>
        /// <param name="longitude">the longitude, in degrees</param>
        public GeoLocation(double latitude, double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
        }

        /// <returns>the latitudinal angle of this location, in degrees.</returns>
        public double GetLatitude()
        {
            return _latitude;
        }

        /// <returns>the longitudinal angle of this location, in degrees.</returns>
        public double GetLongitude()
        {
            return _longitude;
        }

        /// <returns>true, if both latitude and longitude are equal to zero</returns>
        public bool IsZero()
        {
            return _latitude == 0 && _longitude == 0;
        }

        /// <summary>
        /// Converts a decimal degree angle into its corresponding DMS (degrees-minutes-seconds) representation as a string,
        /// of format:
        /// <c>-1° 23' 4.56"</c>
        /// </summary>
        [NotNull]
        public static string DecimalToDegreesMinutesSecondsString(double @decimal)
        {
            var dms = DecimalToDegreesMinutesSeconds(@decimal);
            var format = new DecimalFormat("0.##");
            return string.Format("{0}° {1}' {2}\"", format.Format(dms[0]), format.Format(dms[1]), format.Format(dms[2]));
        }

        /// <summary>
        /// Converts a decimal degree angle into its corresponding DMS (degrees-minutes-seconds) component values, as
        /// a double array.
        /// </summary>
        [NotNull]
        public static double[] DecimalToDegreesMinutesSeconds(double @decimal)
        {
            var d = (int)@decimal;
            var m = Math.Abs((@decimal % 1) * 60);
            var s = (m % 1) * 60;
            return new[] { d, (int)m, s };
        }

        /// <summary>
        /// Converts DMS (degrees-minutes-seconds) rational values, as given in
        /// <see cref="GpsDirectory"/>, into a single value in degrees,
        /// as a double.
        /// </summary>
        [CanBeNull]
        public static double? DegreesMinutesSecondsToDecimal([NotNull] Rational degs, [NotNull] Rational mins, [NotNull] Rational secs, bool isNegative)
        {
            var @decimal = Math.Abs(degs.DoubleValue()) + mins.DoubleValue() / 60.0d + secs.DoubleValue() / 3600.0d;
            if (double.IsNaN(@decimal))
            {
                return null;
            }
            if (isNegative)
            {
                @decimal *= -1;
            }
            return @decimal;
        }

        #region Equality and Hashing

        private bool Equals(GeoLocation other)
        {
            return
                _latitude.Equals(other._latitude) &&
                _longitude.Equals(other._longitude);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is GeoLocation && Equals((GeoLocation)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_latitude.GetHashCode()*397) ^ _longitude.GetHashCode();
            }
        }

        #endregion

        /// <returns>
        /// a string representation of this location, of format:
        /// <c>1.23, 4.56</c>
        /// </returns>
        public override string ToString()
        {
            return _latitude + ", " + _longitude;
        }

        /// <returns>
        /// a string representation of this location, of format:
        /// <c>-1° 23' 4.56", 54° 32' 1.92"</c>
        /// </returns>
        [NotNull]
        public string ToDmsString()
        {
            return DecimalToDegreesMinutesSecondsString(_latitude) + ", " + DecimalToDegreesMinutesSecondsString(_longitude);
        }
    }
}

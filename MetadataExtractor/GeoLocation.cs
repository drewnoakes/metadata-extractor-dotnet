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
using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;

namespace MetadataExtractor
{
    /// <summary>Represents a latitude and longitude pair, giving a position on earth in spherical coordinates.</summary>
    /// <remarks>
    /// Values of latitude and longitude are given in degrees.
    /// <para />
    /// This type is immutable.
    /// </remarks>
    public sealed class GeoLocation
    {
        /// <summary>
        /// Initialises an instance of <see cref="GeoLocation"/>.
        /// </summary>
        /// <param name="latitude">the latitude, in degrees</param>
        /// <param name="longitude">the longitude, in degrees</param>
        public GeoLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <value>the latitudinal angle of this location, in degrees.</value>
        public double Latitude { get; }

        /// <value>the longitudinal angle of this location, in degrees.</value>
        public double Longitude { get; }

        /// <value>true, if both latitude and longitude are equal to zero</value>
        public bool IsZero => Latitude == 0 && Longitude == 0;

        #region Static helpers/factories

        /// <summary>
        /// Converts a decimal degree angle into its corresponding DMS (degrees-minutes-seconds) representation as a string,
        /// of format:
        /// <c>-1° 23' 4.56"</c>
        /// </summary>
        [NotNull, Pure]
        public static string DecimalToDegreesMinutesSecondsString(double value)
        {
            var dms = DecimalToDegreesMinutesSeconds(value);
            return $"{dms[0]:0.##}\u00b0 {dms[1]:0.##}' {dms[2]:0.##}\"";
        }

        /// <summary>
        /// Converts a decimal degree angle into its corresponding DMS (degrees-minutes-seconds) component values, as
        /// a double array.
        /// </summary>
        [NotNull, Pure]
        public static double[] DecimalToDegreesMinutesSeconds(double value)
        {
            var d = (int)value;
            var m = Math.Abs((value%1)*60);
            var s = (m%1)*60;
            return new[] { d, (int)m, s };
        }

        /// <summary>
        /// Converts DMS (degrees-minutes-seconds) rational values, as given in
        /// <see cref="GpsDirectory"/>, into a single value in degrees,
        /// as a double.
        /// </summary>
        [CanBeNull, Pure]
        public static double? DegreesMinutesSecondsToDecimal(Rational degs, Rational mins, Rational secs, bool isNegative)
        {
            var value = Math.Abs(degs.ToDouble()) + mins.ToDouble()/60.0d + secs.ToDouble()/3600.0d;
            if (double.IsNaN(value))
                return null;
            if (isNegative)
                value *= -1;
            return value;
        }

        #endregion

        #region Equality and Hashing

        private bool Equals([NotNull] GeoLocation other) => Latitude.Equals(other.Latitude) &&
                                                  Longitude.Equals(other.Longitude);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is GeoLocation location && Equals(location);
        }

        public override int GetHashCode() => unchecked((Latitude.GetHashCode()*397) ^ Longitude.GetHashCode());

        #endregion

        #region ToString

        /// <returns>
        /// Returns a string representation of this object, of format:
        /// <c>1.23, 4.56</c>
        /// </returns>
        public override string ToString() => Latitude + ", " + Longitude;

        /// <returns>
        /// a string representation of this location, of format:
        /// <c>-1° 23' 4.56", 54° 32' 1.92"</c>
        /// </returns>
        [NotNull, Pure]
        public string ToDmsString() => DecimalToDegreesMinutesSecondsString(Latitude) + ", " + DecimalToDegreesMinutesSecondsString(Longitude);

        #endregion
    }
}

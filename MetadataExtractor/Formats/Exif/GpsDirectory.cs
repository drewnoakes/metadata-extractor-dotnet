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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>Describes Exif tags that contain Global Positioning System (GPS) data.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class GpsDirectory : ExifDirectoryBase
    {
        /// <summary>GPS tag version GPSVersionID 0 0 BYTE 4</summary>
        public const int TagVersionId = 0x0000;

        /// <summary>North or South Latitude GPSLatitudeRef 1 1 ASCII 2</summary>
        public const int TagLatitudeRef = 0x0001;

        /// <summary>Latitude GPSLatitude 2 2 RATIONAL 3</summary>
        public const int TagLatitude = 0x0002;

        /// <summary>East or West Longitude GPSLongitudeRef 3 3 ASCII 2</summary>
        public const int TagLongitudeRef = 0x0003;

        /// <summary>Longitude GPSLongitude 4 4 RATIONAL 3</summary>
        public const int TagLongitude = 0x0004;

        /// <summary>Altitude reference GPSAltitudeRef 5 5 BYTE 1</summary>
        public const int TagAltitudeRef = 0x0005;

        /// <summary>Altitude GPSAltitude 6 6 RATIONAL 1</summary>
        public const int TagAltitude = 0x0006;

        /// <summary>GPS time (atomic clock) GPSTimeStamp 7 7 RATIONAL 3</summary>
        public const int TagTimeStamp = 0x0007;

        /// <summary>GPS satellites used for measurement GPSSatellites 8 8 ASCII Any</summary>
        public const int TagSatellites = 0x0008;

        /// <summary>GPS receiver status GPSStatus 9 9 ASCII 2</summary>
        public const int TagStatus = 0x0009;

        /// <summary>GPS measurement mode GPSMeasureMode 10 A ASCII 2</summary>
        public const int TagMeasureMode = 0x000A;

        /// <summary>Measurement precision GPSDOP 11 B RATIONAL 1</summary>
        public const int TagDop = 0x000B;

        /// <summary>Speed unit GPSSpeedRef 12 C ASCII 2</summary>
        public const int TagSpeedRef = 0x000C;

        /// <summary>Speed of GPS receiver GPSSpeed 13 D RATIONAL 1</summary>
        public const int TagSpeed = 0x000D;

        /// <summary>Reference for direction of movement GPSTrackRef 14 E ASCII 2</summary>
        public const int TagTrackRef = 0x000E;

        /// <summary>Direction of movement GPSTrack 15 F RATIONAL 1</summary>
        public const int TagTrack = 0x000F;

        /// <summary>Reference for direction of image GPSImgDirectionRef 16 10 ASCII 2</summary>
        public const int TagImgDirectionRef = 0x0010;

        /// <summary>Direction of image GPSImgDirection 17 11 RATIONAL 1</summary>
        public const int TagImgDirection = 0x0011;

        /// <summary>Geodetic survey data used GPSMapDatum 18 12 ASCII Any</summary>
        public const int TagMapDatum = 0x0012;

        /// <summary>Reference for latitude of destination GPSDestLatitudeRef 19 13 ASCII 2</summary>
        public const int TagDestLatitudeRef = 0x0013;

        /// <summary>Latitude of destination GPSDestLatitude 20 14 RATIONAL 3</summary>
        public const int TagDestLatitude = 0x0014;

        /// <summary>Reference for longitude of destination GPSDestLongitudeRef 21 15 ASCII 2</summary>
        public const int TagDestLongitudeRef = 0x0015;

        /// <summary>Longitude of destination GPSDestLongitude 22 16 RATIONAL 3</summary>
        public const int TagDestLongitude = 0x0016;

        /// <summary>Reference for bearing of destination GPSDestBearingRef 23 17 ASCII 2</summary>
        public const int TagDestBearingRef = 0x0017;

        /// <summary>Bearing of destination GPSDestBearing 24 18 RATIONAL 1</summary>
        public const int TagDestBearing = 0x0018;

        /// <summary>Reference for distance to destination GPSDestDistanceRef 25 19 ASCII 2</summary>
        public const int TagDestDistanceRef = 0x0019;

        /// <summary>Distance to destination GPSDestDistance 26 1A RATIONAL 1</summary>
        public const int TagDestDistance = 0x001A;

        /// <summary>Values of "GPS", "CELLID", "WLAN" or "MANUAL" by the EXIF spec.</summary>
        public const int TagProcessingMethod = 0x001B;

        public const int TagAreaInformation = 0x001C;

        public const int TagDateStamp = 0x001D;

        public const int TagDifferential = 0x001E;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>();

        static GpsDirectory()
        {
            AddExifTagNames(_tagNameMap);

            // NOTE there is overlap between the base Exif tags and the GPS tags,
            // so we add the GPS tags after to ensure they're kept.

            _tagNameMap[TagVersionId] = "GPS Version ID";
            _tagNameMap[TagLatitudeRef] = "GPS Latitude Ref";
            _tagNameMap[TagLatitude] = "GPS Latitude";
            _tagNameMap[TagLongitudeRef] = "GPS Longitude Ref";
            _tagNameMap[TagLongitude] = "GPS Longitude";
            _tagNameMap[TagAltitudeRef] = "GPS Altitude Ref";
            _tagNameMap[TagAltitude] = "GPS Altitude";
            _tagNameMap[TagTimeStamp] = "GPS Time-Stamp";
            _tagNameMap[TagSatellites] = "GPS Satellites";
            _tagNameMap[TagStatus] = "GPS Status";
            _tagNameMap[TagMeasureMode] = "GPS Measure Mode";
            _tagNameMap[TagDop] = "GPS DOP";
            _tagNameMap[TagSpeedRef] = "GPS Speed Ref";
            _tagNameMap[TagSpeed] = "GPS Speed";
            _tagNameMap[TagTrackRef] = "GPS Track Ref";
            _tagNameMap[TagTrack] = "GPS Track";
            _tagNameMap[TagImgDirectionRef] = "GPS Img Direction Ref";
            _tagNameMap[TagImgDirection] = "GPS Img Direction";
            _tagNameMap[TagMapDatum] = "GPS Map Datum";
            _tagNameMap[TagDestLatitudeRef] = "GPS Dest Latitude Ref";
            _tagNameMap[TagDestLatitude] = "GPS Dest Latitude";
            _tagNameMap[TagDestLongitudeRef] = "GPS Dest Longitude Ref";
            _tagNameMap[TagDestLongitude] = "GPS Dest Longitude";
            _tagNameMap[TagDestBearingRef] = "GPS Dest Bearing Ref";
            _tagNameMap[TagDestBearing] = "GPS Dest Bearing";
            _tagNameMap[TagDestDistanceRef] = "GPS Dest Distance Ref";
            _tagNameMap[TagDestDistance] = "GPS Dest Distance";
            _tagNameMap[TagProcessingMethod] = "GPS Processing Method";
            _tagNameMap[TagAreaInformation] = "GPS Area Information";
            _tagNameMap[TagDateStamp] = "GPS Date Stamp";
            _tagNameMap[TagDifferential] = "GPS Differential";
        }

        public GpsDirectory()
        {
            SetDescriptor(new GpsDescriptor(this));
        }

        public override string Name => "GPS";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the latitude and longitude
        /// at which this image was captured.
        /// </summary>
        /// <returns>The geographical location of this image, if possible, otherwise <c>null</c>.</returns>
        [CanBeNull]
        public GeoLocation GetGeoLocation()
        {
            var latitudes = this.GetRationalArray(TagLatitude);
            var longitudes = this.GetRationalArray(TagLongitude);
            var latitudeRef = this.GetString(TagLatitudeRef);
            var longitudeRef = this.GetString(TagLongitudeRef);

            // Make sure we have the required values
            if (latitudes == null || latitudes.Length != 3)
                return null;
            if (longitudes == null || longitudes.Length != 3)
                return null;
            if (latitudeRef == null || longitudeRef == null)
                return null;

            var lat = GeoLocation.DegreesMinutesSecondsToDecimal(latitudes[0],  latitudes[1],  latitudes[2],  latitudeRef.Equals("S", StringComparison.OrdinalIgnoreCase));
            var lon = GeoLocation.DegreesMinutesSecondsToDecimal(longitudes[0], longitudes[1], longitudes[2], longitudeRef.Equals("W", StringComparison.OrdinalIgnoreCase));

            // This can return null, in cases where the conversion was not possible
            if (lat == null || lon == null)
                return null;

            return new GeoLocation((double)lat, (double)lon);
        }

        /// <summary>
        /// Parses values for <see cref="TagDateStamp"/> and <see cref="TagTimeStamp"/> to produce a single
        /// <see cref="DateTime"/> value representing when this image was captured according to the GPS unit.
        /// </summary>
        public bool TryGetGpsDate(out DateTime date)
        {
            if (!this.TryGetDateTime(TagDateStamp, out date))
                return false;

            var timeComponents = this.GetRationalArray(TagTimeStamp);

            if (timeComponents == null || timeComponents.Length != 3)
                return false;

            date = date
                .AddHours(timeComponents[0].ToDouble())
                .AddMinutes(timeComponents[1].ToDouble())
                .AddSeconds(timeComponents[2].ToDouble());

            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            return true;
        }
    }
}

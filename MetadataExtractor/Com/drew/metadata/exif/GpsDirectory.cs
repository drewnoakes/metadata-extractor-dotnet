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
using System.Collections.Generic;
using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Exif
{
    /// <summary>Describes Exif tags that contain Global Positioning System (GPS) data.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class GpsDirectory : ExifDirectoryBase
    {
        /// <summary>GPS tag version GPSVersionID 0 0 BYTE 4</summary>
        public const int TagVersionId = unchecked(0x0000);

        /// <summary>North or South Latitude GPSLatitudeRef 1 1 ASCII 2</summary>
        public const int TagLatitudeRef = unchecked(0x0001);

        /// <summary>Latitude GPSLatitude 2 2 RATIONAL 3</summary>
        public const int TagLatitude = unchecked(0x0002);

        /// <summary>East or West Longitude GPSLongitudeRef 3 3 ASCII 2</summary>
        public const int TagLongitudeRef = unchecked(0x0003);

        /// <summary>Longitude GPSLongitude 4 4 RATIONAL 3</summary>
        public const int TagLongitude = unchecked(0x0004);

        /// <summary>Altitude reference GPSAltitudeRef 5 5 BYTE 1</summary>
        public const int TagAltitudeRef = unchecked(0x0005);

        /// <summary>Altitude GPSAltitude 6 6 RATIONAL 1</summary>
        public const int TagAltitude = unchecked(0x0006);

        /// <summary>GPS time (atomic clock) GPSTimeStamp 7 7 RATIONAL 3</summary>
        public const int TagTimeStamp = unchecked(0x0007);

        /// <summary>GPS satellites used for measurement GPSSatellites 8 8 ASCII Any</summary>
        public const int TagSatellites = unchecked(0x0008);

        /// <summary>GPS receiver status GPSStatus 9 9 ASCII 2</summary>
        public const int TagStatus = unchecked(0x0009);

        /// <summary>GPS measurement mode GPSMeasureMode 10 A ASCII 2</summary>
        public const int TagMeasureMode = unchecked(0x000A);

        /// <summary>Measurement precision GPSDOP 11 B RATIONAL 1</summary>
        public const int TagDop = unchecked(0x000B);

        /// <summary>Speed unit GPSSpeedRef 12 C ASCII 2</summary>
        public const int TagSpeedRef = unchecked(0x000C);

        /// <summary>Speed of GPS receiver GPSSpeed 13 D RATIONAL 1</summary>
        public const int TagSpeed = unchecked(0x000D);

        /// <summary>Reference for direction of movement GPSTrackRef 14 E ASCII 2</summary>
        public const int TagTrackRef = unchecked(0x000E);

        /// <summary>Direction of movement GPSTrack 15 F RATIONAL 1</summary>
        public const int TagTrack = unchecked(0x000F);

        /// <summary>Reference for direction of image GPSImgDirectionRef 16 10 ASCII 2</summary>
        public const int TagImgDirectionRef = unchecked(0x0010);

        /// <summary>Direction of image GPSImgDirection 17 11 RATIONAL 1</summary>
        public const int TagImgDirection = unchecked(0x0011);

        /// <summary>Geodetic survey data used GPSMapDatum 18 12 ASCII Any</summary>
        public const int TagMapDatum = unchecked(0x0012);

        /// <summary>Reference for latitude of destination GPSDestLatitudeRef 19 13 ASCII 2</summary>
        public const int TagDestLatitudeRef = unchecked(0x0013);

        /// <summary>Latitude of destination GPSDestLatitude 20 14 RATIONAL 3</summary>
        public const int TagDestLatitude = unchecked(0x0014);

        /// <summary>Reference for longitude of destination GPSDestLongitudeRef 21 15 ASCII 2</summary>
        public const int TagDestLongitudeRef = unchecked(0x0015);

        /// <summary>Longitude of destination GPSDestLongitude 22 16 RATIONAL 3</summary>
        public const int TagDestLongitude = unchecked(0x0016);

        /// <summary>Reference for bearing of destination GPSDestBearingRef 23 17 ASCII 2</summary>
        public const int TagDestBearingRef = unchecked(0x0017);

        /// <summary>Bearing of destination GPSDestBearing 24 18 RATIONAL 1</summary>
        public const int TagDestBearing = unchecked(0x0018);

        /// <summary>Reference for distance to destination GPSDestDistanceRef 25 19 ASCII 2</summary>
        public const int TagDestDistanceRef = unchecked(0x0019);

        /// <summary>Distance to destination GPSDestDistance 26 1A RATIONAL 1</summary>
        public const int TagDestDistance = unchecked(0x001A);

        /// <summary>Values of "GPS", "CELLID", "WLAN" or "MANUAL" by the EXIF spec.</summary>
        public const int TagProcessingMethod = unchecked(0x001B);

        public const int TagAreaInformation = unchecked(0x001C);

        public const int TagDateStamp = unchecked(0x001D);

        public const int TagDifferential = unchecked(0x001E);

        [NotNull] private static readonly Dictionary<int?, string> TagNameMap = new Dictionary<int?, string>();

        static GpsDirectory()
        {
            AddExifTagNames(TagNameMap);
            TagNameMap[TagVersionId] = "GPS Version ID";
            TagNameMap[TagLatitudeRef] = "GPS Latitude Ref";
            TagNameMap[TagLatitude] = "GPS Latitude";
            TagNameMap[TagLongitudeRef] = "GPS Longitude Ref";
            TagNameMap[TagLongitude] = "GPS Longitude";
            TagNameMap[TagAltitudeRef] = "GPS Altitude Ref";
            TagNameMap[TagAltitude] = "GPS Altitude";
            TagNameMap[TagTimeStamp] = "GPS Time-Stamp";
            TagNameMap[TagSatellites] = "GPS Satellites";
            TagNameMap[TagStatus] = "GPS Status";
            TagNameMap[TagMeasureMode] = "GPS Measure Mode";
            TagNameMap[TagDop] = "GPS DOP";
            TagNameMap[TagSpeedRef] = "GPS Speed Ref";
            TagNameMap[TagSpeed] = "GPS Speed";
            TagNameMap[TagTrackRef] = "GPS Track Ref";
            TagNameMap[TagTrack] = "GPS Track";
            TagNameMap[TagImgDirectionRef] = "GPS Img Direction Ref";
            TagNameMap[TagImgDirection] = "GPS Img Direction";
            TagNameMap[TagMapDatum] = "GPS Map Datum";
            TagNameMap[TagDestLatitudeRef] = "GPS Dest Latitude Ref";
            TagNameMap[TagDestLatitude] = "GPS Dest Latitude";
            TagNameMap[TagDestLongitudeRef] = "GPS Dest Longitude Ref";
            TagNameMap[TagDestLongitude] = "GPS Dest Longitude";
            TagNameMap[TagDestBearingRef] = "GPS Dest Bearing Ref";
            TagNameMap[TagDestBearing] = "GPS Dest Bearing";
            TagNameMap[TagDestDistanceRef] = "GPS Dest Distance Ref";
            TagNameMap[TagDestDistance] = "GPS Dest Distance";
            TagNameMap[TagProcessingMethod] = "GPS Processing Method";
            TagNameMap[TagAreaInformation] = "GPS Area Information";
            TagNameMap[TagDateStamp] = "GPS Date Stamp";
            TagNameMap[TagDifferential] = "GPS Differential";
        }

        public GpsDirectory()
        {
            SetDescriptor(new GpsDescriptor(this));
        }

        public override string GetName()
        {
            return "GPS";
        }

        protected override Dictionary<int?, string> GetTagNameMap()
        {
            return TagNameMap;
        }

        /// <summary>
        /// Parses various tags in an attempt to obtain a single object representing the latitude and longitude
        /// at which this image was captured.
        /// </summary>
        /// <returns>The geographical location of this image, if possible, otherwise null</returns>
        [CanBeNull]
        public GeoLocation GetGeoLocation()
        {
            var latitudes = GetRationalArray(TagLatitude);
            var longitudes = GetRationalArray(TagLongitude);
            var latitudeRef = GetString(TagLatitudeRef);
            var longitudeRef = GetString(TagLongitudeRef);
            // Make sure we have the required values
            if (latitudes == null || latitudes.Length != 3)
            {
                return null;
            }
            if (longitudes == null || longitudes.Length != 3)
            {
                return null;
            }
            if (latitudeRef == null || longitudeRef == null)
            {
                return null;
            }
            var lat = GeoLocation.DegreesMinutesSecondsToDecimal(latitudes[0], latitudes[1], latitudes[2], latitudeRef.Equals ("S", StringComparison.CurrentCultureIgnoreCase));
            var lon = GeoLocation.DegreesMinutesSecondsToDecimal(longitudes[0], longitudes[1], longitudes[2], longitudeRef.Equals ("W", StringComparison.CurrentCultureIgnoreCase));
            // This can return null, in cases where the conversion was not possible
            if (lat == null || lon == null)
            {
                return null;
            }
            return new GeoLocation((double)lat, (double)lon);
        }
    }
}

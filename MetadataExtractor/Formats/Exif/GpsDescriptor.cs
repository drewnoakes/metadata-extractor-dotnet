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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="GpsDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class GpsDescriptor : TagDescriptor<GpsDirectory>
    {
        public GpsDescriptor([NotNull] GpsDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case GpsDirectory.TagVersionId:
                    return GetGpsVersionIdDescription();
                case GpsDirectory.TagAltitude:
                    return GetGpsAltitudeDescription();
                case GpsDirectory.TagAltitudeRef:
                    return GetGpsAltitudeRefDescription();
                case GpsDirectory.TagStatus:
                    return GetGpsStatusDescription();
                case GpsDirectory.TagMeasureMode:
                    return GetGpsMeasureModeDescription();
                case GpsDirectory.TagSpeedRef:
                    return GetGpsSpeedRefDescription();
                case GpsDirectory.TagTrackRef:
                case GpsDirectory.TagImgDirectionRef:
                case GpsDirectory.TagDestBearingRef:
                    return GetGpsDirectionReferenceDescription(tagType);
                case GpsDirectory.TagTrack:
                case GpsDirectory.TagImgDirection:
                case GpsDirectory.TagDestBearing:
                    return GetGpsDirectionDescription(tagType);
                case GpsDirectory.TagDestDistanceRef:
                    return GetGpsDestinationReferenceDescription();
                case GpsDirectory.TagTimeStamp:
                    return GetGpsTimeStampDescription();
                case GpsDirectory.TagLongitude:
                    // three rational numbers -- displayed in HH"MM"SS.ss
                    return GetGpsLongitudeDescription();
                case GpsDirectory.TagLatitude:
                    // three rational numbers -- displayed in HH"MM"SS.ss
                    return GetGpsLatitudeDescription();
                case GpsDirectory.TagDifferential:
                    return GetGpsDifferentialDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        private string GetGpsVersionIdDescription()
        {
            return GetVersionBytesDescription(GpsDirectory.TagVersionId, 1);
        }

        [CanBeNull]
        public string GetGpsLatitudeDescription()
        {
            var location = Directory.GetGeoLocation();
            return location == null ? null : GeoLocation.DecimalToDegreesMinutesSecondsString(location.Latitude);
        }

        [CanBeNull]
        public string GetGpsLongitudeDescription()
        {
            var location = Directory.GetGeoLocation();
            return location == null ? null : GeoLocation.DecimalToDegreesMinutesSecondsString(location.Longitude);
        }

        [CanBeNull]
        public string GetGpsTimeStampDescription()
        {
            // time in hour, min, sec
            var timeComponents = Directory.GetRationalArray(GpsDirectory.TagTimeStamp);
            return timeComponents == null
                ? null
                : $"{timeComponents[0].ToInt32():D2}:{timeComponents[1].ToInt32():D2}:{timeComponents[2].ToDouble():00.000} UTC";
        }

        [CanBeNull]
        public string GetGpsDestinationReferenceDescription()
        {
            var value = Directory.GetString(GpsDirectory.TagDestDistanceRef);
            if (value == null)
                return null;

            switch (value.Trim().ToUpper())
            {
                case "K":
                    return "kilometers";
                case "M":
                    return "miles";
                case "N":
                    return "knots";
            }

            return "Unknown (" + value.Trim() + ")";
        }

        [CanBeNull]
        public string GetGpsDirectionDescription(int tagType)
        {
            if (!Directory.TryGetRational(tagType, out Rational angle))
                return null;
            // provide a decimal version of rational numbers in the description, to avoid strings like "35334/199 degrees"
            return angle.ToDouble().ToString("0.##") + " degrees";
        }

        [CanBeNull]
        public string GetGpsDirectionReferenceDescription(int tagType)
        {
            var value = Directory.GetString(tagType);
            if (value == null)
                return null;

            switch (value.Trim().ToUpper())
            {
                case "T":
                    return "True direction";
                case "M":
                    return "Magnetic direction";
            }

            return "Unknown (" + value.Trim() + ")";
        }

        [CanBeNull]
        public string GetGpsSpeedRefDescription()
        {
            var value = Directory.GetString(GpsDirectory.TagSpeedRef);
            if (value == null)
                return null;

            switch (value.Trim().ToUpper())
            {
                case "K":
                    return "kph";
                case "M":
                    return "mph";
                case "N":
                    return "knots";
            }

            return "Unknown (" + value.Trim() + ")";
        }

        [CanBeNull]
        public string GetGpsMeasureModeDescription()
        {
            var value = Directory.GetString(GpsDirectory.TagMeasureMode);
            if (value == null)
                return null;

            switch (value.Trim())
            {
                case "2":
                    return "2-dimensional measurement";
                case "3":
                    return "3-dimensional measurement";
            }
            return "Unknown (" + value.Trim() + ")";
        }


        [CanBeNull]
        public string GetGpsStatusDescription()
        {
            var value = Directory.GetString(GpsDirectory.TagStatus);
            if (value == null)
                return null;

            switch (value.Trim().ToUpper())
            {
                case "A":
                    return "Active (Measurement in progress)";
                case "V":
                    return "Void (Measurement Interoperability)";
            }

            return "Unknown (" + value.Trim() + ")";
        }

        [CanBeNull]
        public string GetGpsAltitudeRefDescription()
        {
            return GetIndexedDescription(GpsDirectory.TagAltitudeRef,
                "Sea level", "Below sea level");
        }

        [CanBeNull]
        public string GetGpsAltitudeDescription()
        {
            if (!Directory.TryGetRational(GpsDirectory.TagAltitude, out Rational value))
                return null;
            return value.ToInt32() + " metres";
        }

        [CanBeNull]
        public string GetGpsDifferentialDescription()
        {
            return GetIndexedDescription(GpsDirectory.TagDifferential,
                "No Correction", "Differential Corrected");
        }

        [CanBeNull]
        public string GetDegreesMinutesSecondsDescription()
        {
            var location = Directory.GetGeoLocation();
            return location?.ToDmsString();
        }
    }
}

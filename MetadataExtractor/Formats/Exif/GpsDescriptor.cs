// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="GpsDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class GpsDescriptor : TagDescriptor<GpsDirectory>
    {
        public GpsDescriptor(GpsDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
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
                case GpsDirectory.TagDop:
                    return GetGpsDopDescription();
                case GpsDirectory.TagSpeedRef:
                    return GetGpsSpeedRefDescription();
                case GpsDirectory.TagSpeed:
                    return GetGpsSpeedDescription();
                case GpsDirectory.TagTrackRef:
                case GpsDirectory.TagImgDirectionRef:
                case GpsDirectory.TagDestBearingRef:
                    return GetGpsDirectionReferenceDescription(tagType);
                case GpsDirectory.TagTrack:
                case GpsDirectory.TagImgDirection:
                case GpsDirectory.TagDestBearing:
                    return GetGpsDirectionDescription(tagType);
                case GpsDirectory.TagDestLatitude:
                    return GetGpsDestLatitudeDescription();
                case GpsDirectory.TagDestLongitude:
                    return GetGpsDestLongitudeDescription();
                case GpsDirectory.TagDestDistanceRef:
                    return GetGpsDestinationReferenceDescription();
                case GpsDirectory.TagDestDistance:
                    return GetGpsDestDistanceDescription();
                case GpsDirectory.TagTimeStamp:
                    return GetGpsTimeStampDescription();
                case GpsDirectory.TagLongitude:
                    // three rational numbers -- displayed in HH"MM"SS.ss
                    return GetGpsLongitudeDescription();
                case GpsDirectory.TagLatitude:
                    // three rational numbers -- displayed in HH"MM"SS.ss
                    return GetGpsLatitudeDescription();
                case GpsDirectory.TagProcessingMethod:
                    return GetGpsProcessingMethodDescription();
                case GpsDirectory.TagAreaInformation:
                    return GetGpsAreaInformationDescription();
                case GpsDirectory.TagDifferential:
                    return GetGpsDifferentialDescription();
                case GpsDirectory.TagHPositioningError:
                    return GetGpsHPositioningErrorDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        private string? GetGpsVersionIdDescription()
        {
            return GetVersionBytesDescription(GpsDirectory.TagVersionId, 1);
        }

        public string? GetGpsLatitudeDescription()
        {
            var location = Directory.GetGeoLocation();
            return location == null ? null : GeoLocation.DecimalToDegreesMinutesSecondsString(location.Latitude);
        }

        public string? GetGpsLongitudeDescription()
        {
            var location = Directory.GetGeoLocation();
            return location == null ? null : GeoLocation.DecimalToDegreesMinutesSecondsString(location.Longitude);
        }

        public string? GetGpsTimeStampDescription()
        {
            // time in hour, min, sec
            var timeComponents = Directory.GetRationalArray(GpsDirectory.TagTimeStamp);
            return timeComponents == null
                ? null
                : $"{timeComponents[0].ToInt32():D2}:{timeComponents[1].ToInt32():D2}:{timeComponents[2].ToDouble():00.000} UTC";
        }

        public string? GetGpsDestLatitudeDescription()
        {
            var latitudes = Directory.GetRationalArray(GpsDirectory.TagDestLatitude);
            var latitudeRef = Directory.GetString(GpsDirectory.TagDestLatitudeRef);

            if (latitudes == null || latitudes.Length != 3 || latitudeRef == null)
                return null;

            var lat = GeoLocation.DegreesMinutesSecondsToDecimal(
                latitudes[0], latitudes[1], latitudes[2], latitudeRef.Equals("S", StringComparison.OrdinalIgnoreCase));

            return lat == null ? null : GeoLocation.DecimalToDegreesMinutesSecondsString((double)lat);
        }

        public string? GetGpsDestLongitudeDescription()
        {
            var longitudes = Directory.GetRationalArray(GpsDirectory.TagDestLongitude);
            var longitudeRef = Directory.GetString(GpsDirectory.TagDestLongitudeRef);

            if (longitudes == null || longitudes.Length != 3 || longitudeRef == null)
                return null;

            var lon = GeoLocation.DegreesMinutesSecondsToDecimal(
                longitudes[0], longitudes[1], longitudes[2], longitudeRef.Equals("S", StringComparison.OrdinalIgnoreCase));

            return lon == null ? null : GeoLocation.DecimalToDegreesMinutesSecondsString((double)lon);
        }

        public string? GetGpsDestinationReferenceDescription()
        {
            var value = Directory.GetString(GpsDirectory.TagDestDistanceRef);
            if (value == null)
                return null;

            return (value.Trim().ToUpper()) switch
            {
                "K" => "kilometers",
                "M" => "miles",
                "N" => "knots",

                _ => "Unknown (" + value.Trim() + ")",
            };
        }

        public string? GetGpsDestDistanceDescription()
        {
            if (!Directory.TryGetRational(GpsDirectory.TagDestDistance, out Rational value))
                return null;

            var unit = GetGpsDestinationReferenceDescription();
            return string.Format("{0} {1}", value.ToDouble().ToString("0.##"), unit == null ? "unit" : unit.ToLower());
        }

        public string? GetGpsDirectionDescription(int tagType)
        {
            if (!Directory.TryGetRational(tagType, out Rational angle))
                return null;
            // provide a decimal version of rational numbers in the description, to avoid strings like "35334/199 degrees"
            return angle.ToDouble().ToString("0.##") + " degrees";
        }

        public string? GetGpsDirectionReferenceDescription(int tagType)
        {
            var value = Directory.GetString(tagType);
            if (value == null)
                return null;

            return (value.Trim().ToUpper()) switch
            {
                "T" => "True direction",
                "M" => "Magnetic direction",

                _ => "Unknown (" + value.Trim() + ")",
            };
        }

        public string? GetGpsDopDescription()
        {
            if (!Directory.TryGetRational(GpsDirectory.TagDop, out Rational value))
                return null;
            return $"{value.ToDouble():0.##}";
        }

        public string? GetGpsSpeedRefDescription()
        {
            var value = Directory.GetString(GpsDirectory.TagSpeedRef);
            if (value == null)
                return null;

            return (value.Trim().ToUpper()) switch
            {
                "K" => "km/h",
                "M" => "mph",
                "N" => "knots",

                _ => "Unknown (" + value.Trim() + ")",
            };
        }

        public string? GetGpsSpeedDescription()
        {
            if (!Directory.TryGetRational(GpsDirectory.TagSpeed, out Rational value))
                return null;

            var unit = GetGpsSpeedRefDescription();
            return string.Format("{0} {1}", value.ToDouble().ToString("0.##"), unit == null ? "unit" : unit.ToLower());
        }

        public string? GetGpsMeasureModeDescription()
        {
            var value = Directory.GetString(GpsDirectory.TagMeasureMode);
            if (value == null)
                return null;

            return (value.Trim()) switch
            {
                "2" => "2-dimensional measurement",
                "3" => "3-dimensional measurement",
                _ => "Unknown (" + value.Trim() + ")",
            };
        }


        public string? GetGpsStatusDescription()
        {
            var value = Directory.GetString(GpsDirectory.TagStatus);
            if (value == null)
                return null;

            return (value.Trim().ToUpper()) switch
            {
                "A" => "Active (Measurement in progress)",
                "V" => "Void (Measurement Interoperability)",

                _ => "Unknown (" + value.Trim() + ")",
            };
        }

        public string? GetGpsAltitudeRefDescription()
        {
            return GetIndexedDescription(GpsDirectory.TagAltitudeRef,
                "Sea level", "Below sea level");
        }

        public string? GetGpsAltitudeDescription()
        {
            if (!Directory.TryGetRational(GpsDirectory.TagAltitude, out Rational value))
                return null;
            return $"{value.ToDouble():0.##} metres";
        }

        public string? GetGpsProcessingMethodDescription()
        {
            return GetEncodedTextDescription(GpsDirectory.TagProcessingMethod);
        }

        public string? GetGpsAreaInformationDescription()
        {
            return GetEncodedTextDescription(GpsDirectory.TagAreaInformation);
        }

        public string? GetGpsDifferentialDescription()
        {
            return GetIndexedDescription(GpsDirectory.TagDifferential,
                "No Correction", "Differential Corrected");
        }

        public string? GetGpsHPositioningErrorDescription()
        {
            if (!Directory.TryGetRational(GpsDirectory.TagHPositioningError, out Rational value))
                return null;
            return $"{value.ToDouble():0.##} metres";
        }

        public string? GetDegreesMinutesSecondsDescription()
        {
            var location = Directory.GetGeoLocation();
            return location?.ToDmsString();
        }
    }
}

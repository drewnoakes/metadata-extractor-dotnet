// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="GpsDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class GpsDescriptor(GpsDirectory directory)
        : TagDescriptor<GpsDirectory>(directory)
    {
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
            if (Directory.TryGetGeoLocation(out GeoLocation geoLocation))
            {
                return GeoLocation.DecimalToDegreesMinutesSecondsString(geoLocation.Latitude);
            }

            return null;
        }

        public string? GetGpsLongitudeDescription()
        {
            if (Directory.TryGetGeoLocation(out GeoLocation geoLocation))
            {
                return GeoLocation.DecimalToDegreesMinutesSecondsString(geoLocation.Longitude);
            }

            return null;
        }

        public string? GetGpsTimeStampDescription()
        {
            // time in hour, min, sec
            var timeComponents = Directory.GetRationalArray(GpsDirectory.TagTimeStamp);
            return timeComponents is null
                ? null
                : $"{timeComponents[0].ToInt32():D2}:{timeComponents[1].ToInt32():D2}:{timeComponents[2].ToDouble():00.000} UTC";
        }

        public string? GetGpsDestLatitudeDescription()
        {
            return GetGeoLocationDimension(GpsDirectory.TagDestLatitude, GpsDirectory.TagDestLatitudeRef, "S");
        }

        public string? GetGpsDestLongitudeDescription()
        {
            return GetGeoLocationDimension(GpsDirectory.TagDestLongitude, GpsDirectory.TagDestLongitudeRef, "W");
        }

        private string? GetGeoLocationDimension(int tagValue, int tagRef, string positiveRef)
        {
            var values = Directory.GetRationalArray(tagValue);
            var @ref = Directory.GetString(tagRef);

            if (values is null || values.Length != 3 || @ref is null)
                return null;

            var dec = GeoLocation.DegreesMinutesSecondsToDecimal(
                values[0], values[1], values[2], @ref.Equals(positiveRef, StringComparison.OrdinalIgnoreCase));

            return dec == null ? null : GeoLocation.DecimalToDegreesMinutesSecondsString((double)dec);
        }

        public string? GetGpsDestinationReferenceDescription()
        {
            var value = Directory.GetString(GpsDirectory.TagDestDistanceRef);
            if (value is null)
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
            return string.Format("{0} {1}", value.ToDouble().ToString("0.##"), unit is null ? "unit" : unit.ToLower());
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
            if (value is null)
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
            if (value is null)
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
            return string.Format("{0} {1}", value.ToDouble().ToString("0.##"), unit is null ? "unit" : unit.ToLower());
        }

        public string? GetGpsMeasureModeDescription()
        {
            var value = Directory.GetString(GpsDirectory.TagMeasureMode);
            if (value is null)
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
            if (value is null)
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
            if (Directory.TryGetGeoLocation(out GeoLocation geoLocation))
                return geoLocation.ToDmsString();
            return null;
        }
    }
}

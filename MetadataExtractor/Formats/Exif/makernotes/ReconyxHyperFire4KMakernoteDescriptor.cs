// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Globalization;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="ReconyxHyperFire4KMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>Reconyx HyperFire 4K uses a fixed makernote block. Tag values are the byte offset within the makernote structure.</remarks>
    public sealed class ReconyxHyperFire4KMakernoteDescriptor(ReconyxHyperFire4KMakernoteDirectory directory)
        : TagDescriptor<ReconyxHyperFire4KMakernoteDirectory>(directory)
    {
        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case ReconyxHyperFire4KMakernoteDirectory.TagMakernoteIdentifier:
                    return Directory.GetString(tagType);

                case ReconyxHyperFire4KMakernoteDirectory.TagAggregateMakernoteVersion:
                case ReconyxHyperFire4KMakernoteDirectory.TagAggregateMakernoteSize:
                case ReconyxHyperFire4KMakernoteDirectory.TagMakernoteInfoVersion:
                    return Directory.GetUInt32(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagMakernoteInfoSize:
                    return Directory.GetUInt16(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareMajor:
                case ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareMinor:
                case ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareBuildMonth:
                case ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareBuildDay:
                    return Directory.GetByte(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareBuildYear:
                    return Directory.GetUInt16(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareRevision:
                    var cameraRevision = Directory.GetByte(tagType);
                    return cameraRevision != 0 ? ((char)cameraRevision).ToString() : "";

                case ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareMajor:
                case ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareMinor:
                case ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareBuildMonth:
                case ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareBuildDay:
                    return Directory.GetByte(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareBuildYear:
                    return Directory.GetUInt16(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareRevision:
                    var uibRevision = Directory.GetByte(tagType);
                    return uibRevision != 0 ? ((char)uibRevision).ToString() : "";

                case ReconyxHyperFire4KMakernoteDirectory.TagEventType:
                    var eventType = Directory.GetByte(tagType);
                    return eventType != 0 ? ((char)eventType).ToString() : "";

                case ReconyxHyperFire4KMakernoteDirectory.TagEventSequenceNumber:
                case ReconyxHyperFire4KMakernoteDirectory.TagMaxEventSequenceNumber:
                    return Directory.GetByte(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagEventNumber:
                    return Directory.GetUInt32(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagTimeSeconds:
                case ReconyxHyperFire4KMakernoteDirectory.TagTimeMinutes:
                case ReconyxHyperFire4KMakernoteDirectory.TagTimeHours:
                case ReconyxHyperFire4KMakernoteDirectory.TagDateDay:
                case ReconyxHyperFire4KMakernoteDirectory.TagDateMonth:
                    return Directory.GetByte(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagDateYear:
                    return Directory.GetUInt16(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagDateDayOfWeek:
                    return GetIndexedDescription(tagType, "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday");

                case ReconyxHyperFire4KMakernoteDirectory.TagMoonPhase:
                    return GetIndexedDescription(tagType, "New", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full", "Waning Gibbous", "Last Quarter", "Waning Crescent");

                case ReconyxHyperFire4KMakernoteDirectory.TagTemperatureFahrenheit:
                    return $"{Directory.GetInt16(tagType)}°F";

                case ReconyxHyperFire4KMakernoteDirectory.TagTemperatureCelsius:
                    return $"{Directory.GetInt16(tagType)}°C";

                case ReconyxHyperFire4KMakernoteDirectory.TagContrast:
                case ReconyxHyperFire4KMakernoteDirectory.TagBrightness:
                case ReconyxHyperFire4KMakernoteDirectory.TagSharpness:
                case ReconyxHyperFire4KMakernoteDirectory.TagSaturation:
                    return Directory.GetUInt16(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagFlash:
                    return GetIndexedDescription(tagType, "Off", "On");

                case ReconyxHyperFire4KMakernoteDirectory.TagAmbientLightReading:
                    return Directory.GetUInt32(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagMotionSensorSensitivity:
                    return Directory.GetUInt16(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagBatteryVoltageInstantaneous:
                case ReconyxHyperFire4KMakernoteDirectory.TagBatteryVoltageAverage:
                    return Directory.GetDouble(tagType).ToString("0.000");

                case ReconyxHyperFire4KMakernoteDirectory.TagBatteryType:
                    return Directory.GetUInt16(tagType).ToString();

                case ReconyxHyperFire4KMakernoteDirectory.TagUserLabel:
                case ReconyxHyperFire4KMakernoteDirectory.TagCameraSerialNumber:
                    return Directory.GetString(tagType);

                case ReconyxHyperFire4KMakernoteDirectory.TagRECNXDirectoryNumber:
                case ReconyxHyperFire4KMakernoteDirectory.TagFileNumber:
                    return Directory.GetUInt16(tagType).ToString();

                default:
                    return base.GetDescription(tagType);
            }
        }
    }
}
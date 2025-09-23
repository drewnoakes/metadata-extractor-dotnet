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
            return tagType switch
            {
                ReconyxHyperFire4KMakernoteDirectory.TagMakernoteIdentifier => GetMakernoteIdentifierDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagAggregateMakernoteVersion => GetAggregateMakernoteVersionDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagAggregateMakernoteSize => GetAggregateMakernoteSizeDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagMakernoteInfoVersion => GetMakernoteInfoVersionDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagMakernoteInfoSize => GetMakernoteInfoSizeDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareMajor => GetCameraFirmwareMajorDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareMinor => GetCameraFirmwareMinorDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareBuildYear => GetCameraFirmwareBuildYearDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareBuildMonth => GetCameraFirmwareBuildMonthDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareBuildDay => GetCameraFirmwareBuildDayDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareRevision => GetCameraFirmwareRevisionDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareMajor => GetUIBFirmwareMajorDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareMinor => GetUIBFirmwareMinorDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareBuildYear => GetUIBFirmwareBuildYearDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareBuildMonth => GetUIBFirmwareBuildMonthDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareBuildDay => GetUIBFirmwareBuildDayDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareRevision => GetUIBFirmwareRevisionDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagEventType => GetEventTypeDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagEventSequenceNumber => GetEventSequenceNumberDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagMaxEventSequenceNumber => GetMaxEventSequenceNumberDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagEventNumber => GetEventNumberDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagTimeSeconds => GetTimeSecondsDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagTimeMinutes => GetTimeMinutesDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagTimeHours => GetTimeHoursDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagDateDay => GetDateDayDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagDateMonth => GetDateMonthDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagDateYear => GetDateYearDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagDateDayOfWeek => GetDateDayOfWeekDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagMoonPhase => GetMoonPhaseDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagTemperatureFahrenheit => GetTemperatureFahrenheitDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagTemperatureCelsius => GetTemperatureCelsiusDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagContrast => GetContrastDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagBrightness => GetBrightnessDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagSharpness => GetSharpnessDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagSaturation => GetSaturationDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagFlash => GetFlashDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagAmbientLightReading => GetAmbientLightReadingDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagMotionSensorSensitivity => GetMotionSensorSensitivityDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagBatteryVoltageInstantaneous => GetBatteryVoltageInstantaneousDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagBatteryVoltageAverage => GetBatteryVoltageAverageDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagBatteryType => GetBatteryTypeDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagUserLabel => GetUserLabelDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagCameraSerialNumber => GetCameraSerialNumberDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagRECNXDirectoryNumber => GetRECNXDirectoryNumberDescription(),
                ReconyxHyperFire4KMakernoteDirectory.TagFileNumber => GetFileNumberDescription(),
                _ => base.GetDescription(tagType)
            };
        }

        public string? GetMakernoteIdentifierDescription()
        {
            return Directory.GetString(ReconyxHyperFire4KMakernoteDirectory.TagMakernoteIdentifier);
        }

        public string? GetAggregateMakernoteVersionDescription()
        {
            return Directory.GetUInt32(ReconyxHyperFire4KMakernoteDirectory.TagAggregateMakernoteVersion).ToString();
        }

        public string? GetAggregateMakernoteSizeDescription()
        {
            return Directory.GetUInt32(ReconyxHyperFire4KMakernoteDirectory.TagAggregateMakernoteSize).ToString();
        }

        public string? GetMakernoteInfoVersionDescription()
        {
            return Directory.GetUInt32(ReconyxHyperFire4KMakernoteDirectory.TagMakernoteInfoVersion).ToString();
        }

        public string? GetMakernoteInfoSizeDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagMakernoteInfoSize).ToString();
        }

        public string? GetCameraFirmwareMajorDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareMajor).ToString();
        }

        public string? GetCameraFirmwareMinorDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareMinor).ToString();
        }

        public string? GetCameraFirmwareBuildYearDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareBuildYear).ToString();
        }

        public string? GetCameraFirmwareBuildMonthDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareBuildMonth).ToString();
        }

        public string? GetCameraFirmwareBuildDayDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareBuildDay).ToString();
        }

        public string? GetCameraFirmwareRevisionDescription()
        {
            var cameraRevision = Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagCameraFirmwareRevision);
            return cameraRevision != 0 ? ((char)cameraRevision).ToString() : "";
        }

        public string? GetUIBFirmwareMajorDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareMajor).ToString();
        }

        public string? GetUIBFirmwareMinorDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareMinor).ToString();
        }

        public string? GetUIBFirmwareBuildYearDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareBuildYear).ToString();
        }

        public string? GetUIBFirmwareBuildMonthDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareBuildMonth).ToString();
        }

        public string? GetUIBFirmwareBuildDayDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareBuildDay).ToString();
        }

        public string? GetUIBFirmwareRevisionDescription()
        {
            var uibRevision = Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagUIBFirmwareRevision);
            return uibRevision != 0 ? ((char)uibRevision).ToString() : "";
        }

        public string? GetEventTypeDescription()
        {
            var eventType = Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagEventType);
            return eventType != 0 ? ((char)eventType).ToString() : "";
        }

        public string? GetEventSequenceNumberDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagEventSequenceNumber).ToString();
        }

        public string? GetMaxEventSequenceNumberDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagMaxEventSequenceNumber).ToString();
        }

        public string? GetEventNumberDescription()
        {
            return Directory.GetUInt32(ReconyxHyperFire4KMakernoteDirectory.TagEventNumber).ToString();
        }

        public string? GetTimeSecondsDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagTimeSeconds).ToString();
        }

        public string? GetTimeMinutesDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagTimeMinutes).ToString();
        }

        public string? GetTimeHoursDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagTimeHours).ToString();
        }

        public string? GetDateDayDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagDateDay).ToString();
        }

        public string? GetDateMonthDescription()
        {
            return Directory.GetByte(ReconyxHyperFire4KMakernoteDirectory.TagDateMonth).ToString();
        }

        public string? GetDateYearDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagDateYear).ToString();
        }

        public string? GetDateDayOfWeekDescription()
        {
            return GetIndexedDescription(ReconyxHyperFire4KMakernoteDirectory.TagDateDayOfWeek, "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday");
        }

        public string? GetMoonPhaseDescription()
        {
            return GetIndexedDescription(ReconyxHyperFire4KMakernoteDirectory.TagMoonPhase, "New", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full", "Waning Gibbous", "Last Quarter", "Waning Crescent");
        }

        public string? GetTemperatureFahrenheitDescription()
        {
            return $"{Directory.GetInt16(ReconyxHyperFire4KMakernoteDirectory.TagTemperatureFahrenheit)}°F";
        }

        public string? GetTemperatureCelsiusDescription()
        {
            return $"{Directory.GetInt16(ReconyxHyperFire4KMakernoteDirectory.TagTemperatureCelsius)}°C";
        }

        public string? GetContrastDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagContrast).ToString();
        }

        public string? GetBrightnessDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagBrightness).ToString();
        }

        public string? GetSharpnessDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagSharpness).ToString();
        }

        public string? GetSaturationDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagSaturation).ToString();
        }

        public string? GetFlashDescription()
        {
            return GetIndexedDescription(ReconyxHyperFire4KMakernoteDirectory.TagFlash, "Off", "On");
        }

        public string? GetAmbientLightReadingDescription()
        {
            return Directory.GetUInt32(ReconyxHyperFire4KMakernoteDirectory.TagAmbientLightReading).ToString();
        }

        public string? GetMotionSensorSensitivityDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagMotionSensorSensitivity).ToString();
        }

        public string? GetBatteryVoltageInstantaneousDescription()
        {
            return Directory.GetDouble(ReconyxHyperFire4KMakernoteDirectory.TagBatteryVoltageInstantaneous).ToString("0.000");
        }

        public string? GetBatteryVoltageAverageDescription()
        {
            return Directory.GetDouble(ReconyxHyperFire4KMakernoteDirectory.TagBatteryVoltageAverage).ToString("0.000");
        }

        public string? GetBatteryTypeDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagBatteryType).ToString();
        }

        public string? GetUserLabelDescription()
        {
            return Directory.GetString(ReconyxHyperFire4KMakernoteDirectory.TagUserLabel);
        }

        public string? GetCameraSerialNumberDescription()
        {
            return Directory.GetString(ReconyxHyperFire4KMakernoteDirectory.TagCameraSerialNumber);
        }

        public string? GetRECNXDirectoryNumberDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagRECNXDirectoryNumber).ToString();
        }

        public string? GetFileNumberDescription()
        {
            return Directory.GetUInt16(ReconyxHyperFire4KMakernoteDirectory.TagFileNumber).ToString();
        }
    }
}
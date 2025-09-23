// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Globalization;
using static MetadataExtractor.Formats.Exif.Makernotes.ReconyxHyperFire4KMakernoteDirectory;

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
                TagMakernoteIdentifier => GetMakernoteIdentifierDescription(),
                TagAggregateMakernoteVersion => GetAggregateMakernoteVersionDescription(),
                TagAggregateMakernoteSize => GetAggregateMakernoteSizeDescription(),
                TagMakernoteInfoVersion => GetMakernoteInfoVersionDescription(),
                TagMakernoteInfoSize => GetMakernoteInfoSizeDescription(),
                TagCameraFirmwareMajor => GetCameraFirmwareMajorDescription(),
                TagCameraFirmwareMinor => GetCameraFirmwareMinorDescription(),
                TagCameraFirmwareBuildYear => GetCameraFirmwareBuildYearDescription(),
                TagCameraFirmwareBuildMonth => GetCameraFirmwareBuildMonthDescription(),
                TagCameraFirmwareBuildDay => GetCameraFirmwareBuildDayDescription(),
                TagCameraFirmwareRevision => GetCameraFirmwareRevisionDescription(),
                TagUIBFirmwareMajor => GetUIBFirmwareMajorDescription(),
                TagUIBFirmwareMinor => GetUIBFirmwareMinorDescription(),
                TagUIBFirmwareBuildYear => GetUIBFirmwareBuildYearDescription(),
                TagUIBFirmwareBuildMonth => GetUIBFirmwareBuildMonthDescription(),
                TagUIBFirmwareBuildDay => GetUIBFirmwareBuildDayDescription(),
                TagUIBFirmwareRevision => GetUIBFirmwareRevisionDescription(),
                TagEventType => GetEventTypeDescription(),
                TagEventSequenceNumber => GetEventSequenceNumberDescription(),
                TagMaxEventSequenceNumber => GetMaxEventSequenceNumberDescription(),
                TagEventNumber => GetEventNumberDescription(),
                TagTimeSeconds => GetTimeSecondsDescription(),
                TagTimeMinutes => GetTimeMinutesDescription(),
                TagTimeHours => GetTimeHoursDescription(),
                TagDateDay => GetDateDayDescription(),
                TagDateMonth => GetDateMonthDescription(),
                TagDateYear => GetDateYearDescription(),
                TagDateDayOfWeek => GetDateDayOfWeekDescription(),
                TagMoonPhase => GetMoonPhaseDescription(),
                TagTemperatureFahrenheit => GetTemperatureFahrenheitDescription(),
                TagTemperatureCelsius => GetTemperatureCelsiusDescription(),
                TagContrast => GetContrastDescription(),
                TagBrightness => GetBrightnessDescription(),
                TagSharpness => GetSharpnessDescription(),
                TagSaturation => GetSaturationDescription(),
                TagFlash => GetFlashDescription(),
                TagAmbientLightReading => GetAmbientLightReadingDescription(),
                TagMotionSensorSensitivity => GetMotionSensorSensitivityDescription(),
                TagBatteryVoltageInstantaneous => GetBatteryVoltageInstantaneousDescription(),
                TagBatteryVoltageAverage => GetBatteryVoltageAverageDescription(),
                TagBatteryType => GetBatteryTypeDescription(),
                TagUserLabel => GetUserLabelDescription(),
                TagCameraSerialNumber => GetCameraSerialNumberDescription(),
                TagRECNXDirectoryNumber => GetRECNXDirectoryNumberDescription(),
                TagFileNumber => GetFileNumberDescription(),
                _ => base.GetDescription(tagType)
            };
        }

        public string? GetMakernoteIdentifierDescription()
        {
            return Directory.GetString(TagMakernoteIdentifier);
        }

        public string? GetAggregateMakernoteVersionDescription()
        {
            return Directory.TryGetUInt32(TagAggregateMakernoteVersion, out var value) ? value.ToString() : null;
        }

        public string? GetAggregateMakernoteSizeDescription()
        {
            return Directory.TryGetUInt32(TagAggregateMakernoteSize, out var value) ? value.ToString() : null;
        }

        public string? GetMakernoteInfoVersionDescription()
        {
            return Directory.TryGetUInt32(TagMakernoteInfoVersion, out var value) ? value.ToString() : null;
        }

        public string? GetMakernoteInfoSizeDescription()
        {
            return Directory.TryGetUInt16(TagMakernoteInfoSize, out var value) ? value.ToString() : null;
        }

        public string? GetCameraFirmwareMajorDescription()
        {
            return Directory.TryGetByte(TagCameraFirmwareMajor, out var value) ? value.ToString() : null;
        }

        public string? GetCameraFirmwareMinorDescription()
        {
            return Directory.TryGetByte(TagCameraFirmwareMinor, out var value) ? value.ToString() : null;
        }

        public string? GetCameraFirmwareBuildYearDescription()
        {
            return Directory.TryGetUInt16(TagCameraFirmwareBuildYear, out var value) ? value.ToString() : null;
        }

        public string? GetCameraFirmwareBuildMonthDescription()
        {
            return Directory.TryGetByte(TagCameraFirmwareBuildMonth, out var value) ? value.ToString() : null;
        }

        public string? GetCameraFirmwareBuildDayDescription()
        {
            return Directory.TryGetByte(TagCameraFirmwareBuildDay, out var value) ? value.ToString() : null;
        }

        public string? GetCameraFirmwareRevisionDescription()
        {
            if (!Directory.TryGetByte(TagCameraFirmwareRevision, out var value))
                return null;
            return value != 0 ? ((char)value).ToString() : "";
        }

        public string? GetUIBFirmwareMajorDescription()
        {
            return Directory.TryGetByte(TagUIBFirmwareMajor, out var value) ? value.ToString() : null;
        }

        public string? GetUIBFirmwareMinorDescription()
        {
            return Directory.TryGetByte(TagUIBFirmwareMinor, out var value) ? value.ToString() : null;
        }

        public string? GetUIBFirmwareBuildYearDescription()
        {
            return Directory.TryGetUInt16(TagUIBFirmwareBuildYear, out var value) ? value.ToString() : null;
        }

        public string? GetUIBFirmwareBuildMonthDescription()
        {
            return Directory.TryGetByte(TagUIBFirmwareBuildMonth, out var value) ? value.ToString() : null;
        }

        public string? GetUIBFirmwareBuildDayDescription()
        {
            return Directory.TryGetByte(TagUIBFirmwareBuildDay, out var value) ? value.ToString() : null;
        }

        public string? GetUIBFirmwareRevisionDescription()
        {
            if (!Directory.TryGetByte(TagUIBFirmwareRevision, out var value))
                return null;
            return value != 0 ? ((char)value).ToString() : "";
        }

        public string? GetEventTypeDescription()
        {
            if (!Directory.TryGetByte(TagEventType, out var value))
                return null;
            
            return value != 0 ? ((char)value) switch
            {
                'C' => "CodeLoc Not Entered",
                'E' => "External Sensor",
                'L' => "Cell Live View", 
                'M' => "Motion Sensor",
                'S' => "Cell Status",
                'T' => "Time Lapse",
                _ => ((char)value).ToString()
            } : "";
        }

        public string? GetEventSequenceNumberDescription()
        {
            return Directory.TryGetByte(TagEventSequenceNumber, out var value) ? value.ToString() : null;
        }

        public string? GetMaxEventSequenceNumberDescription()
        {
            return Directory.TryGetByte(TagMaxEventSequenceNumber, out var value) ? value.ToString() : null;
        }

        public string? GetEventNumberDescription()
        {
            return Directory.TryGetUInt32(TagEventNumber, out var value) ? value.ToString() : null;
        }

        public string? GetTimeSecondsDescription()
        {
            return Directory.TryGetByte(TagTimeSeconds, out var value) ? value.ToString() : null;
        }

        public string? GetTimeMinutesDescription()
        {
            return Directory.TryGetByte(TagTimeMinutes, out var value) ? value.ToString() : null;
        }

        public string? GetTimeHoursDescription()
        {
            return Directory.TryGetByte(TagTimeHours, out var value) ? value.ToString() : null;
        }

        public string? GetDateDayDescription()
        {
            return Directory.TryGetByte(TagDateDay, out var value) ? value.ToString() : null;
        }

        public string? GetDateMonthDescription()
        {
            return Directory.TryGetByte(TagDateMonth, out var value) ? value.ToString() : null;
        }

        public string? GetDateYearDescription()
        {
            return Directory.TryGetUInt16(TagDateYear, out var value) ? value.ToString() : null;
        }

        public string? GetDateDayOfWeekDescription()
        {
            return GetIndexedDescription(TagDateDayOfWeek, 1, "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday");
        }

        public string? GetMoonPhaseDescription()
        {
            return GetIndexedDescription(TagMoonPhase, "New", "New Crescent", "First Quarter", "Waxing Gibbous", "Full", "Waning Gibbous", "Last Quarter", "Old Crescent");
        }

        public string? GetTemperatureFahrenheitDescription()
        {
            return Directory.TryGetInt16(TagTemperatureFahrenheit, out var value) ? $"{value}°F" : null;
        }

        public string? GetTemperatureCelsiusDescription()
        {
            return Directory.TryGetInt16(TagTemperatureCelsius, out var value) ? $"{value}°C" : null;
        }

        public string? GetContrastDescription()
        {
            return Directory.TryGetUInt16(TagContrast, out var value) ? value.ToString() : null;
        }

        public string? GetBrightnessDescription()
        {
            return Directory.TryGetUInt16(TagBrightness, out var value) ? value.ToString() : null;
        }

        public string? GetSharpnessDescription()
        {
            return Directory.TryGetUInt16(TagSharpness, out var value) ? value.ToString() : null;
        }

        public string? GetSaturationDescription()
        {
            return Directory.TryGetUInt16(TagSaturation, out var value) ? value.ToString() : null;
        }

        public string? GetFlashDescription()
        {
            return GetIndexedDescription(TagFlash, "Off", "On");
        }

        public string? GetAmbientLightReadingDescription()
        {
            return Directory.TryGetUInt32(TagAmbientLightReading, out var value) ? value.ToString() : null;
        }

        public string? GetMotionSensorSensitivityDescription()
        {
            return Directory.TryGetUInt16(TagMotionSensorSensitivity, out var value) ? value.ToString() : null;
        }

        public string? GetBatteryVoltageInstantaneousDescription()
        {
            return Directory.TryGetDouble(TagBatteryVoltageInstantaneous, out var value) ? value.ToString("0.000") : null;
        }

        public string? GetBatteryVoltageAverageDescription()
        {
            return Directory.TryGetDouble(TagBatteryVoltageAverage, out var value) ? value.ToString("0.000") : null;
        }

        public string? GetBatteryTypeDescription()
        {
            return GetIndexedDescription(TagBatteryType, 1, "NiMH", "Lithium", "External", "SC10 Solar");
        }

        public string? GetUserLabelDescription()
        {
            return Directory.GetString(TagUserLabel);
        }

        public string? GetCameraSerialNumberDescription()
        {
            return Directory.GetString(TagCameraSerialNumber);
        }

        public string? GetRECNXDirectoryNumberDescription()
        {
            return Directory.TryGetUInt16(TagRECNXDirectoryNumber, out var value) ? value.ToString() : null;
        }

        public string? GetFileNumberDescription()
        {
            return Directory.TryGetUInt16(TagFileNumber, out var value) ? value.ToString() : null;
        }
    }
}
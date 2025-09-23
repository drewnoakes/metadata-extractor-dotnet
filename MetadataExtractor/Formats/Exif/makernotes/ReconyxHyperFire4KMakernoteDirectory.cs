// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Reconyx HyperFire 4K cameras.</summary>
    public class ReconyxHyperFire4KMakernoteDirectory : Directory
    {
        // Offsets based on the HyperFire 4K MakerNote Structure specification
        public const int TagMakernoteIdentifier = 0;           // char[12] - "RECONYXHF4K"
        public const int TagAggregateMakernoteVersion = 12;     // uint32 - Aggregate MakerNote structure version
        public const int TagAggregateMakernoteSize = 16;       // uint32 - Aggregate MakerNote structure size in bytes
        public const int TagMakernoteInfoVersion = 20;         // uint32 - MakerNote INFO structure version
        public const int TagMakernoteInfoSize = 24;            // uint16 - MakerNote INFO structure size in bytes

        public const int TagCameraFirmwareMajor = 26;          // uint8 - Camera firmware major version
        public const int TagCameraFirmwareMinor = 27;          // uint8 - Camera firmware minor version
        public const int TagCameraFirmwareBuildYear = 28;      // uint16 - Camera firmware build year
        public const int TagCameraFirmwareBuildMonth = 30;     // uint8 - Camera firmware build month
        public const int TagCameraFirmwareBuildDay = 31;       // uint8 - Camera firmware build day
        public const int TagCameraFirmwareRevision = 32;       // char - Camera firmware revision

        public const int TagUIBFirmwareMajor = 33;             // uint8 - UIB firmware major version
        public const int TagUIBFirmwareMinor = 34;             // uint8 - UIB firmware minor version
        public const int TagUIBFirmwareBuildYear = 35;         // uint16 - UIB firmware build year
        public const int TagUIBFirmwareBuildMonth = 37;        // uint8 - UIB firmware build month
        public const int TagUIBFirmwareBuildDay = 38;          // uint8 - UIB firmware build day
        public const int TagUIBFirmwareRevision = 39;          // char - UIB firmware revision

        public const int TagEventType = 40;                    // char - Event type
        public const int TagEventSequenceNumber = 41;          // uint8 - Event sequence number
        public const int TagMaxEventSequenceNumber = 42;       // uint8 - Maximum event sequence number
        public const int TagEventNumber = 43;                  // uint32 - Event number

        public const int TagTimeSeconds = 47;                  // uint8 - Time (seconds)
        public const int TagTimeMinutes = 48;                  // uint8 - Time (minutes)
        public const int TagTimeHours = 49;                    // uint8 - Time (hours)
        public const int TagDateDay = 50;                      // uint8 - Date (day)
        public const int TagDateMonth = 51;                    // uint8 - Date (month)
        public const int TagDateYear = 52;                     // uint16 - Date (year)
        public const int TagDateDayOfWeek = 54;                // uint8 - Date (day of week)

        public const int TagMoonPhase = 55;                    // uint8 - Moon phase
        public const int TagTemperatureFahrenheit = 56;        // int16 - Temperature (Fahrenheit)
        public const int TagTemperatureCelsius = 58;           // int16 - Temperature (Celsius)

        public const int TagContrast = 60;                     // uint16 - Contrast
        public const int TagBrightness = 62;                   // uint16 - Brightness
        public const int TagSharpness = 64;                    // uint16 - Sharpness
        public const int TagSaturation = 66;                   // uint16 - Saturation
        public const int TagFlash = 68;                        // uint8 - Flash

        public const int TagAmbientLightReading = 69;          // uint32 - Ambient light reading
        public const int TagMotionSensorSensitivity = 73;      // uint16 - Motion sensor sensitivity
        public const int TagBatteryVoltageInstantaneous = 75;  // uint16 - Battery voltage (instantaneous)
        public const int TagBatteryVoltageAverage = 77;        // uint16 - Battery voltage (average)
        public const int TagBatteryType = 79;                  // uint16 - Battery type

        public const int TagUserLabel = 81;                    // char[51] - User label
        public const int TagCameraSerialNumber = 132;          // char[15] - Camera serial number
        public const int TagRECNXDirectoryNumber = 147;        // uint16 - RECNX directory number
        public const int TagFileNumber = 149;                  // uint16 - File number
        public const int TagReserved = 151;                    // uint8[36] - Reserved for future use

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagMakernoteIdentifier, "Makernote Identifier" },
            { TagAggregateMakernoteVersion, "Aggregate Makernote Version" },
            { TagAggregateMakernoteSize, "Aggregate Makernote Size" },
            { TagMakernoteInfoVersion, "Makernote Info Version" },
            { TagMakernoteInfoSize, "Makernote Info Size" },

            { TagCameraFirmwareMajor, "Camera Firmware Major" },
            { TagCameraFirmwareMinor, "Camera Firmware Minor" },
            { TagCameraFirmwareBuildYear, "Camera Firmware Build Year" },
            { TagCameraFirmwareBuildMonth, "Camera Firmware Build Month" },
            { TagCameraFirmwareBuildDay, "Camera Firmware Build Day" },
            { TagCameraFirmwareRevision, "Camera Firmware Revision" },

            { TagUIBFirmwareMajor, "UIB Firmware Major" },
            { TagUIBFirmwareMinor, "UIB Firmware Minor" },
            { TagUIBFirmwareBuildYear, "UIB Firmware Build Year" },
            { TagUIBFirmwareBuildMonth, "UIB Firmware Build Month" },
            { TagUIBFirmwareBuildDay, "UIB Firmware Build Day" },
            { TagUIBFirmwareRevision, "UIB Firmware Revision" },

            { TagEventType, "Event Type" },
            { TagEventSequenceNumber, "Event Sequence Number" },
            { TagMaxEventSequenceNumber, "Max Event Sequence Number" },
            { TagEventNumber, "Event Number" },

            { TagTimeSeconds, "Time Seconds" },
            { TagTimeMinutes, "Time Minutes" },
            { TagTimeHours, "Time Hours" },
            { TagDateDay, "Date Day" },
            { TagDateMonth, "Date Month" },
            { TagDateYear, "Date Year" },
            { TagDateDayOfWeek, "Date Day of Week" },

            { TagMoonPhase, "Moon Phase" },
            { TagTemperatureFahrenheit, "Temperature Fahrenheit" },
            { TagTemperatureCelsius, "Temperature Celsius" },

            { TagContrast, "Contrast" },
            { TagBrightness, "Brightness" },
            { TagSharpness, "Sharpness" },
            { TagSaturation, "Saturation" },
            { TagFlash, "Flash" },

            { TagAmbientLightReading, "Ambient Light Reading" },
            { TagMotionSensorSensitivity, "Motion Sensor Sensitivity" },
            { TagBatteryVoltageInstantaneous, "Battery Voltage Instantaneous" },
            { TagBatteryVoltageAverage, "Battery Voltage Average" },
            { TagBatteryType, "Battery Type" },

            { TagUserLabel, "User Label" },
            { TagCameraSerialNumber, "Camera Serial Number" },
            { TagRECNXDirectoryNumber, "RECNX Directory Number" },
            { TagFileNumber, "File Number" },
            { TagReserved, "Reserved" }
        };

        public ReconyxHyperFire4KMakernoteDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new ReconyxHyperFire4KMakernoteDescriptor(this));
        }

        public override string Name => "Reconyx HyperFire 4K Makernote";
    }
}
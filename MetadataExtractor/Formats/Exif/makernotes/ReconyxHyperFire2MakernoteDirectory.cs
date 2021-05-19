// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Reconyx HyperFire 2 cameras.</summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ReconyxHyperFire2MakernoteDirectory : Directory
    {
        public const int TagFileNumber = 16;
        public const int TagDirectoryNumber = 18;
        public const int TagFirmwareVersion = 42;
        public const int TagFirmwareDate = 48;
        public const int TagTriggerMode = 52;
        public const int TagSequence = 54;
        public const int TagEventNumber = 58;
        public const int TagDateTimeOriginal = 62;
        public const int TagDayOfWeek = 74;
        public const int TagMoonPhase = 76;
        public const int TagAmbientTemperatureFahrenheit = 78;
        public const int TagAmbientTemperatureCelcius = 80;
        public const int TagContrast = 82;
        public const int TagBrightness = 84;
        public const int TagSharpness = 86;
        public const int TagSaturation = 88;
        public const int TagFlash = 90;

        public const int TagAmbientInfrared = 92;
        public const int TagAmbientLight = 94;

        public const int TagMotionSensitivity = 96;
        public const int TagBatteryVoltage = 98;
        public const int TagBatteryVoltageAvg = 100;
        public const int TagBatteryType = 102;
        public const int TagUserLabel = 104;
        public const int TagSerialNumber = 126;

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
             { TagFileNumber, "File Number" },
             { TagDirectoryNumber, "Directory Number" },
             { TagFirmwareVersion, "Firmware Version" },
             { TagFirmwareDate, "Firmware Date" },
             { TagTriggerMode, "Trigger Mode" },
             { TagSequence, "Sequence" },
             { TagEventNumber, "Event Number" },
             { TagDateTimeOriginal, "Date/Time Original" },
             { TagDayOfWeek, "Day Of Week" },
             { TagMoonPhase, "Moon Phase" },
             { TagAmbientTemperatureFahrenheit, "Ambient Temperature Fahrenheit" },
             { TagAmbientTemperatureCelcius, "Ambient Temperature Celcius" },
             { TagContrast, "Contrast" },
             { TagBrightness, "Brightness" },
             { TagSharpness, "Sharpness" },
             { TagSaturation, "Saturation" },
             { TagFlash, "Flash" },
             { TagAmbientInfrared, "Ambient Infrared" },
             { TagAmbientLight, "Ambient Light" },
             { TagMotionSensitivity, "Motion Sensitivity" },
             { TagBatteryVoltage, "Battery Voltage" },
             { TagBatteryVoltageAvg, "Battery Voltage Average" },
             { TagBatteryType, "Battery Type" },
             { TagUserLabel, "User Label" },
             { TagSerialNumber, "Serial Number" },
        };

        public ReconyxHyperFire2MakernoteDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new ReconyxHyperFire2MakernoteDescriptor(this));
        }

        public override string Name => "Reconyx HyperFire 2 Makernote";
    }
}

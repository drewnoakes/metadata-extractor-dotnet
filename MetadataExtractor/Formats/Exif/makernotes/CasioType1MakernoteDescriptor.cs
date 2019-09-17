// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="CasioType1MakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class CasioType1MakernoteDescriptor : TagDescriptor<CasioType1MakernoteDirectory>
    {
        public CasioType1MakernoteDescriptor(CasioType1MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                CasioType1MakernoteDirectory.TagRecordingMode => GetRecordingModeDescription(),
                CasioType1MakernoteDirectory.TagQuality => GetQualityDescription(),
                CasioType1MakernoteDirectory.TagFocusingMode => GetFocusingModeDescription(),
                CasioType1MakernoteDirectory.TagFlashMode => GetFlashModeDescription(),
                CasioType1MakernoteDirectory.TagFlashIntensity => GetFlashIntensityDescription(),
                CasioType1MakernoteDirectory.TagObjectDistance => GetObjectDistanceDescription(),
                CasioType1MakernoteDirectory.TagWhiteBalance => GetWhiteBalanceDescription(),
                CasioType1MakernoteDirectory.TagDigitalZoom => GetDigitalZoomDescription(),
                CasioType1MakernoteDirectory.TagSharpness => GetSharpnessDescription(),
                CasioType1MakernoteDirectory.TagContrast => GetContrastDescription(),
                CasioType1MakernoteDirectory.TagSaturation => GetSaturationDescription(),
                CasioType1MakernoteDirectory.TagCcdSensitivity => GetCcdSensitivityDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetCcdSensitivityDescription()
        {
            if (!Directory.TryGetInt32(CasioType1MakernoteDirectory.TagCcdSensitivity, out int value))
                return null;
            return value switch
            {
                // these four for QV3000
                64 => "Normal",
                125 => "+1.0",
                250 => "+2.0",
                244 => "+3.0",

                // these two for QV8000/2000
                80 => "Normal (ISO 80 equivalent)",
                100 => "High",

                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetSaturationDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagSaturation,
                "Normal", "Low", "High");
        }

        public string? GetContrastDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagContrast,
                "Normal", "Low", "High");
        }

        public string? GetSharpnessDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagSharpness,
                "Normal", "Soft", "Hard");
        }

        public string? GetDigitalZoomDescription()
        {
            if (!Directory.TryGetInt32(CasioType1MakernoteDirectory.TagDigitalZoom, out int value))
                return null;

            return value switch
            {
                0x10000 => "No digital zoom",
                0x10001 => "2x digital zoom",
                0x20000 => "2x digital zoom",
                0x40000 => "4x digital zoom",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetWhiteBalanceDescription()
        {
            if (!Directory.TryGetInt32(CasioType1MakernoteDirectory.TagWhiteBalance, out int value))
                return null;

            return value switch
            {
                1 => "Auto",
                2 => "Tungsten",
                3 => "Daylight",
                4 => "Florescent",
                5 => "Shade",
                129 => "Manual",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetObjectDistanceDescription()
        {
            if (!Directory.TryGetInt32(CasioType1MakernoteDirectory.TagObjectDistance, out int value))
                return null;
            return value + " mm";
        }

        public string? GetFlashIntensityDescription()
        {
            if (!Directory.TryGetInt32(CasioType1MakernoteDirectory.TagFlashIntensity, out int value))
                return null;

            return value switch
            {
                11 => "Weak",
                13 => "Normal",
                15 => "Strong",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetFlashModeDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagFlashMode,
                1,
                "Auto", "On", "Off", "Red eye reduction");
        }

        public string? GetFocusingModeDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagFocusingMode,
                2,
                "Macro", "Auto focus", "Manual focus", "Infinity");
        }

        public string? GetQualityDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagQuality,
                1,
                "Economy", "Normal", "Fine");
        }

        public string? GetRecordingModeDescription()
        {
            return GetIndexedDescription(CasioType1MakernoteDirectory.TagRecordingMode,
                1,
                "Single shutter", "Panorama", "Night scene", "Portrait", "Landscape");
        }
    }
}

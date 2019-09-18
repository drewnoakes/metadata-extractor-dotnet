// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="PentaxMakernoteDirectory"/>.
    /// <para />
    /// Some information about this makernote taken from here:
    /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/pentax_mn.html
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PentaxMakernoteDescriptor : TagDescriptor<PentaxMakernoteDirectory>
    {
        public PentaxMakernoteDescriptor(PentaxMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                PentaxMakernoteDirectory.TagCaptureMode => GetCaptureModeDescription(),
                PentaxMakernoteDirectory.TagQualityLevel => GetQualityLevelDescription(),
                PentaxMakernoteDirectory.TagFocusMode => GetFocusModeDescription(),
                PentaxMakernoteDirectory.TagFlashMode => GetFlashModeDescription(),
                PentaxMakernoteDirectory.TagWhiteBalance => GetWhiteBalanceDescription(),
                PentaxMakernoteDirectory.TagDigitalZoom => GetDigitalZoomDescription(),
                PentaxMakernoteDirectory.TagSharpness => GetSharpnessDescription(),
                PentaxMakernoteDirectory.TagContrast => GetContrastDescription(),
                PentaxMakernoteDirectory.TagSaturation => GetSaturationDescription(),
                PentaxMakernoteDirectory.TagIsoSpeed => GetIsoSpeedDescription(),
                PentaxMakernoteDirectory.TagColour => GetColourDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetColourDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagColour,
                1,
                "Normal", "Black & White", "Sepia");
        }

        public string? GetIsoSpeedDescription()
        {
            if (!Directory.TryGetInt32(PentaxMakernoteDirectory.TagIsoSpeed, out int value))
                return null;

            return value switch
            {
                10 => "ISO 100",
                16 => "ISO 200",
                100 => "ISO 100",
                200 => "ISO 200",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetSaturationDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagSaturation,
                "Normal", "Low", "High");
        }

        public string? GetContrastDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagContrast,
                "Normal", "Low", "High");
        }

        public string? GetSharpnessDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagSharpness,
                "Normal", "Soft", "Hard");
        }

        public string? GetDigitalZoomDescription()
        {
            if (!Directory.TryGetSingle(PentaxMakernoteDirectory.TagDigitalZoom, out float value))
                return null;
            return value == 0 ? "Off" : value.ToString("0.0###########");
        }

        public string? GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagWhiteBalance,
                "Auto", "Daylight", "Shade", "Tungsten", "Fluorescent", "Manual");
        }

        public string? GetFlashModeDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagFlashMode,
                1,
                "Auto", "Flash On", null, "Flash Off", null, "Red-eye Reduction");
        }

        public string? GetFocusModeDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagFocusMode,
                2,
                "Custom", "Auto");
        }

        public string? GetQualityLevelDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagQualityLevel,
                "Good", "Better", "Best");
        }

        public string? GetCaptureModeDescription()
        {
            return GetIndexedDescription(PentaxMakernoteDirectory.TagCaptureMode,
                "Auto", "Night-scene", "Manual", null, "Multiple");
        }
    }
}

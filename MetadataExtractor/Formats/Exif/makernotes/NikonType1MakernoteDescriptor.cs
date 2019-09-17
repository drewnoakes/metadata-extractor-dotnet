// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="NikonType1MakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Type-1 is for E-Series cameras prior to (not including) E990.  For example: E700, E800, E900,
    /// E900S, E910, E950.
    /// <para />
    /// Makernote starts from ASCII string "Nikon". Data format is the same as IFD, but it starts from
    /// offset 0x08. This is the same as Olympus except start string. Example of actual data
    /// structure is shown below.
    /// <pre><c>
    /// :0000: 4E 69 6B 6F 6E 00 01 00-05 00 02 00 02 00 06 00 Nikon...........
    /// :0010: 00 00 EC 02 00 00 03 00-03 00 01 00 00 00 06 00 ................
    /// </c></pre>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class NikonType1MakernoteDescriptor : TagDescriptor<NikonType1MakernoteDirectory>
    {
        public NikonType1MakernoteDescriptor(NikonType1MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                NikonType1MakernoteDirectory.TagQuality => GetQualityDescription(),
                NikonType1MakernoteDirectory.TagColorMode => GetColorModeDescription(),
                NikonType1MakernoteDirectory.TagImageAdjustment => GetImageAdjustmentDescription(),
                NikonType1MakernoteDirectory.TagCcdSensitivity => GetCcdSensitivityDescription(),
                NikonType1MakernoteDirectory.TagWhiteBalance => GetWhiteBalanceDescription(),
                NikonType1MakernoteDirectory.TagFocus => GetFocusDescription(),
                NikonType1MakernoteDirectory.TagDigitalZoom => GetDigitalZoomDescription(),
                NikonType1MakernoteDirectory.TagConverter => GetConverterDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetConverterDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagConverter,
                "None", "Fisheye converter");
        }

        public string? GetDigitalZoomDescription()
        {
            if (!Directory.TryGetRational(NikonType1MakernoteDirectory.TagDigitalZoom, out Rational value))
                return null;
            return value.Numerator == 0 
                ? "No digital zoom" 
                : value.ToSimpleString() + "x digital zoom";
        }

        public string? GetFocusDescription()
        {
            if (!Directory.TryGetRational(NikonType1MakernoteDirectory.TagFocus, out Rational value))
                return null;
            return value.Numerator == 1 && value.Denominator == 0 
                ? "Infinite" 
                : value.ToSimpleString();
        }

        public string? GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagWhiteBalance,
                "Auto", "Preset", "Daylight", "Incandescence", "Florescence", "Cloudy", "SpeedLight");
        }

        public string? GetCcdSensitivityDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagCcdSensitivity,
                "ISO80", null, "ISO160", null, "ISO320", "ISO100");
        }

        public string? GetImageAdjustmentDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagImageAdjustment,
                "Normal", "Bright +", "Bright -", "Contrast +", "Contrast -");
        }

        public string? GetColorModeDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagColorMode,
                1,
                "Color", "Monochrome");
        }

        public string? GetQualityDescription()
        {
            return GetIndexedDescription(NikonType1MakernoteDirectory.TagQuality,
                1,
                "VGA Basic", "VGA Normal", "VGA Fine", "SXGA Basic", "SXGA Normal", "SXGA Fine");
        }
    }
}

// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="OlympusRawDevelopment2MakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some Description functions and the Filter type list converted from Exiftool version 10.10 created by Phil Harvey
    /// http://www.sno.phy.queensu.ca/~phil/exiftool/
    /// lib\Image\ExifTool\Olympus.pm
    /// </remarks>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class OlympusRawDevelopment2MakernoteDescriptor : TagDescriptor<OlympusRawDevelopment2MakernoteDirectory>
    {
        public OlympusRawDevelopment2MakernoteDescriptor(OlympusRawDevelopment2MakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                OlympusRawDevelopment2MakernoteDirectory.TagRawDevVersion => GetRawDevVersionDescription(),
                OlympusRawDevelopment2MakernoteDirectory.TagRawDevExposureBiasValue => GetRawDevExposureBiasValueDescription(),
                OlympusRawDevelopment2MakernoteDirectory.TagRawDevColorSpace => GetRawDevColorSpaceDescription(),
                OlympusRawDevelopment2MakernoteDirectory.TagRawDevNoiseReduction => GetRawDevNoiseReductionDescription(),
                OlympusRawDevelopment2MakernoteDirectory.TagRawDevEngine => GetRawDevEngineDescription(),
                OlympusRawDevelopment2MakernoteDirectory.TagRawDevPictureMode => GetRawDevPictureModeDescription(),
                OlympusRawDevelopment2MakernoteDirectory.TagRawDevPmBwFilter => GetRawDevPmBwFilterDescription(),
                OlympusRawDevelopment2MakernoteDirectory.TagRawDevPmPictureTone => GetRawDevPmPictureToneDescription(),
                OlympusRawDevelopment2MakernoteDirectory.TagRawDevArtFilter => GetRawDevArtFilterDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetRawDevVersionDescription()
        {
            return GetVersionBytesDescription(OlympusRawDevelopment2MakernoteDirectory.TagRawDevVersion, 4);
        }

        public string? GetRawDevExposureBiasValueDescription()
        {
            return GetIndexedDescription(OlympusRawDevelopment2MakernoteDirectory.TagRawDevExposureBiasValue, 1,
                "Color Temperature", "Gray Point");
        }

        public string? GetRawDevColorSpaceDescription()
        {
            return GetIndexedDescription(OlympusRawDevelopment2MakernoteDirectory.TagRawDevColorSpace,
                "sRGB", "Adobe RGB", "Pro Photo RGB");
        }

        public string? GetRawDevNoiseReductionDescription()
        {
            if (!Directory.TryGetInt32(OlympusRawDevelopment2MakernoteDirectory.TagRawDevNoiseReduction, out int value))
                return null;

            if (value == 0)
                return "(none)";

            var sb = new StringBuilder();
            var v = (ushort)value;

            if ((v        & 1) != 0) sb.Append("Noise Reduction, ");
            if (((v >> 1) & 1) != 0) sb.Append("Noise Filter, ");
            if (((v >> 2) & 1) != 0) sb.Append("Noise Filter (ISO Boost), ");

            return sb.ToString(0, sb.Length - 2);
        }

        public string? GetRawDevEngineDescription()
        {
            return GetIndexedDescription(OlympusRawDevelopment2MakernoteDirectory.TagRawDevEngine,
                "High Speed", "High Function", "Advanced High Speed", "Advanced High Function");
        }

        public string? GetRawDevPictureModeDescription()
        {
            if (!Directory.TryGetInt32(OlympusRawDevelopment2MakernoteDirectory.TagRawDevPictureMode, out int value))
                return null;

            return value switch
            {
                1 => "Vivid",
                2 => "Natural",
                3 => "Muted",
                256 => "Monotone",
                512 => "Sepia",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetRawDevPmBwFilterDescription()
        {
            return GetIndexedDescription(OlympusRawDevelopment2MakernoteDirectory.TagRawDevPmBwFilter, 1,
                "Neutral", "Yellow", "Orange", "Red", "Green");
        }

        public string? GetRawDevPmPictureToneDescription()
        {
            return GetIndexedDescription(OlympusRawDevelopment2MakernoteDirectory.TagRawDevPmPictureTone, 1,
                "Neutral", "Sepia", "Blue", "Purple", "Green");
        }

        public string? GetRawDevArtFilterDescription() => GetFilterDescription(OlympusRawDevelopment2MakernoteDirectory.TagRawDevArtFilter);

        private string? GetFilterDescription(int tagId)
        {
            if (!(Directory.GetObject(tagId) is short[] values) || values.Length == 0)
                return null;

            var sb = new StringBuilder();
            for (var i = 0; i < values.Length; i++)
            {
                if (i == 0)
                    sb.Append(_filters.ContainsKey(values[i]) ? _filters[values[i]] : "[unknown]");
                else
                    sb.Append(values[i]);
                sb.Append("; ");
            }

            return sb.ToString(0, sb.Length - 2);
        }

        // RawDevArtFilter values
        private static readonly Dictionary<int, string> _filters = new Dictionary<int, string>
        {
            { 0, "Off" },
            { 1, "Soft Focus" },
            { 2, "Pop Art" },
            { 3, "Pale & Light Color" },
            { 4, "Light Tone" },
            { 5, "Pin Hole" },
            { 6, "Grainy Film" },
            { 9, "Diorama" },
            { 10, "Cross Process" },
            { 12, "Fish Eye" },
            { 13, "Drawing" },
            { 14, "Gentle Sepia" },
            { 15, "Pale & Light Color II" },
            { 16, "Pop Art II" },
            { 17, "Pin Hole II" },
            { 18, "Pin Hole III" },
            { 19, "Grainy Film II" },
            { 20, "Dramatic Tone" },
            { 21, "Punk" },
            { 22, "Soft Focus 2" },
            { 23, "Sparkle" },
            { 24, "Watercolor" },
            { 25, "Key Line" },
            { 26, "Key Line II" },
            { 27, "Miniature" },
            { 28, "Reflection" },
            { 29, "Fragmented" },
            { 31, "Cross Process II" },
            { 32, "Dramatic Tone II" },
            { 33, "Watercolor I" },
            { 34, "Watercolor II" },
            { 35, "Diorama II" },
            { 36, "Vintage" },
            { 37, "Vintage II" },
            { 38, "Vintage III" },
            { 39, "Partial Color" },
            { 40, "Partial Color II" },
            { 41, "Partial Color III" }
        };
    }
}

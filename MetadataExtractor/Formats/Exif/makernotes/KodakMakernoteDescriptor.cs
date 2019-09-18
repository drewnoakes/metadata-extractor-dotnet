// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="KodakMakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class KodakMakernoteDescriptor : TagDescriptor<KodakMakernoteDirectory>
    {
        public KodakMakernoteDescriptor(KodakMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                KodakMakernoteDirectory.TagQuality => GetQualityDescription(),
                KodakMakernoteDirectory.TagBurstMode => GetBurstModeDescription(),
                KodakMakernoteDirectory.TagShutterMode => GetShutterModeDescription(),
                KodakMakernoteDirectory.TagFocusMode => GetFocusModeDescription(),
                KodakMakernoteDirectory.TagWhiteBalance => GetWhiteBalanceDescription(),
                KodakMakernoteDirectory.TagFlashMode => GetFlashModeDescription(),
                KodakMakernoteDirectory.TagFlashFired => GetFlashFiredDescription(),
                KodakMakernoteDirectory.TagColorMode => GetColorModeDescription(),
                KodakMakernoteDirectory.TagSharpness => GetSharpnessDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetSharpnessDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagSharpness, "Normal");
        }

        public string? GetColorModeDescription()
        {
            if (!Directory.TryGetInt32(KodakMakernoteDirectory.TagColorMode, out int value))
                return null;

            switch (value)
            {
                case 0x0001:
                case 0x2000:
                    return "B&W";
                case 0x0002:
                case 0x4000:
                    return "Sepia";
                case 0x003:
                    return "B&W Yellow Filter";
                case 0x004:
                    return "B&W Red Filter";
                case 0x020:
                    return "Saturated Color";
                case 0x040:
                case 0x200:
                    return "Neutral Color";
                case 0x100:
                    return "Saturated Color";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        public string? GetFlashFiredDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagFlashFired, "No", "Yes");
        }

        public string? GetFlashModeDescription()
        {
            if (!Directory.TryGetInt32(KodakMakernoteDirectory.TagFlashMode, out int value))
                return null;

            switch (value)
            {
                case 0x00:
                    return "Auto";
                case 0x10:
                case 0x01:
                    return "Fill Flash";
                case 0x20:
                case 0x02:
                    return "Off";
                case 0x40:
                case 0x03:
                    return "Red Eye";
                default:
                    return "Unknown (" + value + ")";
            }
        }

        public string? GetWhiteBalanceDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagWhiteBalance,
                "Auto", "Flash", "Tungsten", "Daylight");
        }

        public string? GetFocusModeDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagFocusMode,
                "Normal", null, "Macro");
        }

        public string? GetShutterModeDescription()
        {
            if (!Directory.TryGetInt32(KodakMakernoteDirectory.TagShutterMode, out int value))
                return null;

            return value switch
            {
                0 => "Auto",
                8 => "Aperture Priority",
                32 => "Manual",
                _ => "Unknown (" + value + ")",
            };
        }

        public string? GetBurstModeDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagBurstMode,
                "Off", "On");
        }

        public string? GetQualityDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagQuality,
                1,
                "Fine", "Normal");
        }
    }
}

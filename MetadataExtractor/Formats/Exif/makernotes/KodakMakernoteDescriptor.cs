// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="KodakMakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class KodakMakernoteDescriptor(KodakMakernoteDirectory directory)
        : TagDescriptor<KodakMakernoteDirectory>(directory)
    {
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

            return value switch
            {
                0x0001 or 0x2000 => "B&W",
                0x0002 or 0x4000 => "Sepia",
                0x003 => "B&W Yellow Filter",
                0x004 => "B&W Red Filter",
                0x020 => "Saturated Color",
                0x040 or 0x200 => "Neutral Color",
                0x100 => "Saturated Color",
                _ => $"Unknown ({value})"
            };
        }

        public string? GetFlashFiredDescription()
        {
            return GetIndexedDescription(KodakMakernoteDirectory.TagFlashFired, "No", "Yes");
        }

        public string? GetFlashModeDescription()
        {
            if (!Directory.TryGetInt32(KodakMakernoteDirectory.TagFlashMode, out int value))
                return null;

            return value switch
            {
                0x00 => "Auto",
                0x10 or 0x01 => "Fill Flash",
                0x20 or 0x02 => "Off",
                0x40 or 0x03 => "Red Eye",
                _ => $"Unknown ({value})"
            };
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

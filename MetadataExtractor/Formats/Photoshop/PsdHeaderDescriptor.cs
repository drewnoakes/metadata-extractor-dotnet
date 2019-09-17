// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PsdHeaderDescriptor : TagDescriptor<PsdHeaderDirectory>
    {
        public PsdHeaderDescriptor(PsdHeaderDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                PsdHeaderDirectory.TagChannelCount => GetChannelCountDescription(),
                PsdHeaderDirectory.TagBitsPerChannel => GetBitsPerChannelDescription(),
                PsdHeaderDirectory.TagColorMode => GetColorModeDescription(),
                PsdHeaderDirectory.TagImageHeight => GetImageHeightDescription(),
                PsdHeaderDirectory.TagImageWidth => GetImageWidthDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetChannelCountDescription()
        {
            // Supported range is 1 to 56.
            if (!Directory.TryGetInt32(PsdHeaderDirectory.TagChannelCount, out int value))
                return null;
            return value + " channel" + (value == 1 ? string.Empty : "s");
        }

        public string? GetBitsPerChannelDescription()
        {
            // Supported values are 1, 8, 16 and 32.
            if (!Directory.TryGetInt32(PsdHeaderDirectory.TagBitsPerChannel, out int value))
                return null;
            return value + " bit" + (value == 1 ? string.Empty : "s") + " per channel";
        }

        public string? GetColorModeDescription()
        {
            return GetIndexedDescription(PsdHeaderDirectory.TagColorMode,
                "Bitmap",
                "Grayscale",
                "Indexed",
                "RGB",
                "CMYK",
                null,
                null,
                "Multichannel",
                "Duotone",
                "Lab");
        }

        public string? GetImageHeightDescription()
        {
            if (!Directory.TryGetInt32(PsdHeaderDirectory.TagImageHeight, out int value))
                return null;
            return value + " pixel" + (value == 1 ? string.Empty : "s");
        }

        public string? GetImageWidthDescription()
        {
            if (!Directory.TryGetInt32(PsdHeaderDirectory.TagImageWidth, out int value))
                return null;
            return value + " pixel" + (value == 1 ? string.Empty : "s");
        }
    }
}

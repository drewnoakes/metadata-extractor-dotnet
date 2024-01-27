// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Exif.Makernotes.NikonPictureControl2Directory;

namespace MetadataExtractor.Formats.Exif.Makernotes;

public sealed class NikonPictureControl2Descriptor(NikonPictureControl2Directory directory)
    : TagDescriptor<NikonPictureControl2Directory>(directory)
{
    public override string? GetDescription(int tagType)
    {
        return tagType switch
        {
            TagPictureControlAdjust => GetPictureControlAdjustDescription(),
            TagFilterEffect => GetFilterEffectDescription(),
            TagToningEffect => GetToningEffectDescription(),
            _ => base.GetDescription(tagType)
        };
    }

    public string? GetPictureControlAdjustDescription()
    {
        return GetIndexedDescription(
            TagPictureControlAdjust,
            "Default Settings",
            "Quick Adjust",
            "Full Control");
    }

    public string? GetFilterEffectDescription()
    {
        if (!Directory.TryGetByte(TagFilterEffect, out byte value))
            return null;

        return value switch
        {
            0x80 => "Off",
            0x81 => "Yellow",
            0x82 => "Orange",
            0x83 => "Red",
            0x84 => "Green",
            0xFF => "N/A",
            _ => base.GetDescription(TagFilterEffect)
        };
    }

    public string? GetToningEffectDescription()
    {
        if (!Directory.TryGetByte(TagToningEffect, out byte value))
            return null;

        return value switch
        {
            0x80 => "B&W",
            0x81 => "Sepia",
            0x82 => "Cyanotype",
            0x83 => "Red",
            0x84 => "Yellow",
            0x85 => "Green",
            0x86 => "Blue-green",
            0x87 => "Blue",
            0x88 => "Purple-blue",
            0x89 => "Red-purple",
            0xFF => "N/A",
            _ => base.GetDescription(TagToningEffect)
        };
    }
}

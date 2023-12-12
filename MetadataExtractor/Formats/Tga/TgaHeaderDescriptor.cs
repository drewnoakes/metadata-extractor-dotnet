// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tga;

/// <author>Dmitry Shechtman</author>
public sealed class TgaHeaderDescriptor : TagDescriptor<TgaHeaderDirectory>
{
    public TgaHeaderDescriptor(TgaHeaderDirectory directory)
        : base(directory)
    {
    }

    public override string? GetDescription(int tagType)
    {
        return tagType switch
        {
            TgaHeaderDirectory.TagColormapType => GetColormapTypeDescription(),
            TgaHeaderDirectory.TagDataType => GetDataTypeDescription(),
            TgaHeaderDirectory.TagHorizontalOrder => GetHorizontalOrderDescription(),
            TgaHeaderDirectory.TagVerticalOrder => GetVerticalOrderDescription(),
            _ => base.GetDescription(tagType),
        };
    }

    public string? GetColormapTypeDescription()
    {
        return GetIndexedDescription(TgaHeaderDirectory.TagColormapType, "No color map data included", "Color map data included");
    }

    public string? GetDataTypeDescription()
    {
        if (!Directory.TryGetByte(TgaHeaderDirectory.TagDataType, out var value))
            return null;
        return value switch
        {
            1 => "Uncompressed mapped color",
            2 => "Uncompressed true color",
            3 => "Uncompressed grayscale",
            9 => "RLE mapped color",
            10 => "RLE true color",
            11 => "RLE grayscale",
            _ => null,
        };
    }

    public string? GetHorizontalOrderDescription()
    {
        return GetBooleanDescription(
            TgaHeaderDirectory.TagHorizontalOrder,
            trueValue: "Right to left",
            falseValue: "Left to right");
    }

    public string? GetVerticalOrderDescription()
    {
        return GetBooleanDescription(
            TgaHeaderDirectory.TagVerticalOrder,
            trueValue: "Top to bottom",
            falseValue: "Bottom to top");
    }
}

// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Png;

/// <author>Drew Noakes https://drewnoakes.com</author>
public sealed class PngDirectory : Directory
{
    public const int TagImageWidth = 1;
    public const int TagImageHeight = 2;
    public const int TagBitsPerSample = 3;
    public const int TagColorType = 4;
    public const int TagCompressionType = 5;
    public const int TagFilterMethod = 6;
    public const int TagInterlaceMethod = 7;
    public const int TagPaletteSize = 8;
    public const int TagPaletteHasTransparency = 9;
    public const int TagSrgbRenderingIntent = 10;
    public const int TagGamma = 11;
    public const int TagIccProfileName = 12;
    public const int TagTextualData = 13;
    public const int TagLastModificationTime = 14;
    public const int TagBackgroundColor = 15;
    public const int TagPixelsPerUnitX = 16;
    public const int TagPixelsPerUnitY = 17;
    public const int TagUnitSpecifier = 18;
    public const int TagSignificantBits = 19;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagImageHeight, "Image Height" },
        { TagImageWidth, "Image Width" },
        { TagBitsPerSample, "Bits Per Sample" },
        { TagColorType, "Color Type" },
        { TagCompressionType, "Compression Type" },
        { TagFilterMethod, "Filter Method" },
        { TagInterlaceMethod, "Interlace Method" },
        { TagPaletteSize, "Palette Size" },
        { TagPaletteHasTransparency, "Palette Has Transparency" },
        { TagSrgbRenderingIntent, "sRGB Rendering Intent" },
        { TagGamma, "Image Gamma" },
        { TagIccProfileName, "ICC Profile Name" },
        { TagTextualData, "Textual Data" },
        { TagLastModificationTime, "Last Modification Time" },
        { TagBackgroundColor, "Background Color" },
        { TagPixelsPerUnitX, "Pixels Per Unit X" },
        { TagPixelsPerUnitY, "Pixels Per Unit Y" },
        { TagUnitSpecifier, "Unit Specifier" },
        { TagSignificantBits, "Significant Bits" }
    };

    private readonly PngChunkType _pngChunkType;

    public PngDirectory(PngChunkType pngChunkType) : base(_tagNameMap)
    {
        _pngChunkType = pngChunkType;
        SetDescriptor(new PngDescriptor(this));
    }

    public PngChunkType GetPngChunkType()
    {
        return _pngChunkType;
    }

    public override string Name => "PNG-" + _pngChunkType.Identifier;

}

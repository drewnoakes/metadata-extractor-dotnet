// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Gif;

/// <author>Drew Noakes https://drewnoakes.com</author>
public class GifHeaderDirectory : Directory
{
    public const int TagGifFormatVersion = 1;
    public const int TagImageWidth = 2;
    public const int TagImageHeight = 3;
    public const int TagColorTableSize = 4;
    public const int TagIsColorTableSorted = 5;
    public const int TagBitsPerPixel = 6;
    public const int TagHasGlobalColorTable = 7;
    public const int TagBackgroundColorIndex = 8;
    public const int TagPixelAspectRatio = 9;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagGifFormatVersion, "GIF Format Version" },
        { TagImageHeight, "Image Height" },
        { TagImageWidth, "Image Width" },
        { TagColorTableSize, "Color Table Size" },
        { TagIsColorTableSorted, "Is Color Table Sorted" },
        { TagBitsPerPixel, "Bits per Pixel" },
        { TagHasGlobalColorTable, "Has Global Color Table" },
        { TagBackgroundColorIndex, "Background Color Index" },
        { TagPixelAspectRatio, "Pixel Aspect Ratio" }
    };

    public GifHeaderDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new GifHeaderDescriptor(this));
    }

    public override string Name => "GIF Header";
}

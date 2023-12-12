// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Gif;

/// <author>Drew Noakes https://drewnoakes.com</author>
/// <author>Kevin Mott https://github.com/kwhopper</author>
public class GifImageDirectory : Directory
{
    public const int TagLeft = 1;
    public const int TagTop = 2;
    public const int TagWidth = 3;
    public const int TagHeight = 4;
    public const int TagHasLocalColourTable = 5;
    public const int TagIsInterlaced = 6;
    public const int TagIsColorTableSorted = 7;
    public const int TagLocalColourTableBitsPerPixel = 8;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagLeft, "Left" },
        { TagTop, "Top" },
        { TagWidth, "Width" },
        { TagHeight, "Height" },
        { TagHasLocalColourTable, "Has Local Colour Table" },
        { TagIsInterlaced, "Is Interlaced" },
        { TagIsColorTableSorted, "Is Local Colour Table Sorted" },
        { TagLocalColourTableBitsPerPixel, "Local Colour Table Bits Per Pixel" }
    };

    public GifImageDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new GifImageDescriptor(this));
    }

    public override string Name => "GIF Image";
}

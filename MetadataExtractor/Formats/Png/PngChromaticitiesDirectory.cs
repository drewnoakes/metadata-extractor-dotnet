// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Png;

/// <author>Drew Noakes https://drewnoakes.com</author>
public class PngChromaticitiesDirectory : Directory
{
    public const int TagWhitePointX = 1;
    public const int TagWhitePointY = 2;
    public const int TagRedX = 3;
    public const int TagRedY = 4;
    public const int TagGreenX = 5;
    public const int TagGreenY = 6;
    public const int TagBlueX = 7;
    public const int TagBlueY = 8;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagWhitePointX, "White Point X" },
        { TagWhitePointY, "White Point Y" },
        { TagRedX, "Red X" },
        { TagRedY, "Red Y" },
        { TagGreenX, "Green X" },
        { TagGreenY, "Green Y" },
        { TagBlueX, "Blue X" },
        { TagBlueY, "Blue Y" }
    };

    public PngChromaticitiesDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new TagDescriptor<PngChromaticitiesDirectory>(this));
    }

    public override string Name => "PNG Chromaticities";
}

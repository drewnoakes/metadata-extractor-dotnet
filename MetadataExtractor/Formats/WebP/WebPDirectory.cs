// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.WebP;

/// <author>Drew Noakes https://drewnoakes.com</author>
public class WebPDirectory : Directory
{
    public const int TagImageHeight = 1;
    public const int TagImageWidth = 2;
    public const int TagHasAlpha = 3;
    public const int TagIsAnimation = 4;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagImageHeight, "Image Height" },
        { TagImageWidth, "Image Width" },
        { TagHasAlpha, "Has Alpha" },
        { TagIsAnimation, "Is Animation" }
    };

    public WebPDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new WebPDescriptor(this));
    }

    public override string Name => "WebP";
}

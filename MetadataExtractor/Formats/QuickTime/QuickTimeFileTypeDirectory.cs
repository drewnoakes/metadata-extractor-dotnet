// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime;

public sealed class QuickTimeFileTypeDirectory : Directory
{
    public const int TagMajorBrand = 1;
    public const int TagMinorVersion = 2;
    public const int TagCompatibleBrands = 3;

    public override string Name => "QuickTime File Type";

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagMajorBrand,       "Major Brand" },
        { TagMinorVersion,     "Minor Version" },
        { TagCompatibleBrands, "Compatible Brands" }
    };

    public QuickTimeFileTypeDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new QuickTimeFileTypeDescriptor(this));
    }
}

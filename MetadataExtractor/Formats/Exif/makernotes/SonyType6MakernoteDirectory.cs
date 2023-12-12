// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes;

/// <summary>Describes tags specific to Sony cameras that use the Sony Type 6 makernote tags.</summary>
/// <author>Drew Noakes https://drewnoakes.com</author>
public class SonyType6MakernoteDirectory : Directory
{
    public const int TagMakernoteThumbOffset = 0x0513;
    public const int TagMakernoteThumbLength = 0x0514;
    public const int TagMakernoteThumbVersion = 0x2000;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagMakernoteThumbOffset, "Makernote Thumb Offset" },
        { TagMakernoteThumbLength, "Makernote Thumb Length" },
        { TagMakernoteThumbVersion, "Makernote Thumb Version" }
    };

    public SonyType6MakernoteDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new SonyType6MakernoteDescriptor(this));
    }

    public override string Name => "Sony Makernote";
}

// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif;

/// <summary>Describes Exif tags from the SubIFD directory.</summary>
/// <author>Drew Noakes https://drewnoakes.com</author>
public class ExifSubIfdDirectory : ExifDirectoryBase
{
    /// <summary>This tag is a pointer to the Exif Interop IFD.</summary>
    public const int TagInteropOffset = 0xA005;

    public ExifSubIfdDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new ExifSubIfdDescriptor(this));
    }

    private static readonly Dictionary<int, string> _tagNameMap = new();

    static ExifSubIfdDirectory()
    {
        AddExifTagNames(_tagNameMap);
    }

    public override string Name => "Exif SubIFD";
}

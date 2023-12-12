// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif;

/// <remarks>These tags can be found in Epson proprietary metadata. The index values are 'fake' but
/// chosen specifically to make processing easier</remarks>
/// <author>Kevin Mott https://github.com/kwhopper</author>
/// <author>Drew Noakes https://drewnoakes.com</author>
public class PrintIMDirectory : Directory
{
    public const int TagPrintImVersion = 0x0000;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagPrintImVersion, "PrintIM Version" }
    };

    public PrintIMDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new PrintIMDescriptor(this));
    }

    public override string Name => "PrintIM";
}

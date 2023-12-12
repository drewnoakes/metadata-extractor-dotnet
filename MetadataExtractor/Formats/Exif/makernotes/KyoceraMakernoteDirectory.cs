// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes;

/// <summary>Describes tags specific to Kyocera and Contax cameras.</summary>
/// <author>Drew Noakes https://drewnoakes.com</author>
public class KyoceraMakernoteDirectory : Directory
{
    public const int TagProprietaryThumbnail = 0x0001;
    public const int TagPrintImageMatchingInfo = 0x0E00;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagProprietaryThumbnail, "Proprietary Thumbnail Format Data" },
        { TagPrintImageMatchingInfo, "Print Image Matching (PIM) Info" }
    };

    public KyoceraMakernoteDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new KyoceraMakernoteDescriptor(this));
    }

    public override string Name => "Kyocera/Contax Makernote";
}

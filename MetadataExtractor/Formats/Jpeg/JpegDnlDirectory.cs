// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Jpeg;

/// <summary>Describes tags parsed from JPEG DNL data, holding the image height with information missing from the JPEG SOFx segment</summary>
/// <author>Kevin Mott https://github.com/kwhopper</author>
public class JpegDnlDirectory : Directory
{
    /// <summary>The image's height, gleaned from DNL data instead of an SOFx segment</summary>
    public const int TagImageHeight = 1;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagImageHeight, "Image Height" }
    };

    public JpegDnlDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new JpegDnlDescriptor(this));
    }

    public override string Name => "JPEG DNL";
}

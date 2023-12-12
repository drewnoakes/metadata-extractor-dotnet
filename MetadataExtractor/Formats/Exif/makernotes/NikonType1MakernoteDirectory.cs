// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes;

/// <summary>Describes tags specific to Nikon (type 1) cameras.</summary>
/// <remarks>
/// Describes tags specific to Nikon (type 1) cameras.  Type-1 is for E-Series cameras prior to (not including) E990.
/// There are 3 formats of Nikon's Makernote. Makernote of E700/E800/E900/E900S/E910/E950
/// starts from ASCII string "Nikon". Data format is the same as IFD, but it starts from
/// offset 0x08. This is the same as Olympus except start string. Example of actual data
/// structure is shown below.
/// <pre><c>
/// :0000: 4E 69 6B 6F 6E 00 01 00-05 00 02 00 02 00 06 00 Nikon...........
/// :0010: 00 00 EC 02 00 00 03 00-03 00 01 00 00 00 06 00 ................
/// </c></pre>
/// </remarks>
/// <author>Drew Noakes https://drewnoakes.com</author>
public class NikonType1MakernoteDirectory : Directory
{
    public const int TagUnknown1 = 0x0002;
    public const int TagQuality = 0x0003;
    public const int TagColorMode = 0x0004;
    public const int TagImageAdjustment = 0x0005;
    public const int TagCcdSensitivity = 0x0006;
    public const int TagWhiteBalance = 0x0007;
    public const int TagFocus = 0x0008;
    public const int TagUnknown2 = 0x0009;
    public const int TagDigitalZoom = 0x000A;
    public const int TagConverter = 0x000B;
    public const int TagUnknown3 = 0x0F00;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagCcdSensitivity, "CCD Sensitivity" },
        { TagColorMode, "Color Mode" },
        { TagDigitalZoom, "Digital Zoom" },
        { TagConverter, "Fisheye Converter" },
        { TagFocus, "Focus" },
        { TagImageAdjustment, "Image Adjustment" },
        { TagQuality, "Quality" },
        { TagUnknown1, "Makernote Unknown 1" },
        { TagUnknown2, "Makernote Unknown 2" },
        { TagUnknown3, "Makernote Unknown 3" },
        { TagWhiteBalance, "White Balance" }
    };

    public NikonType1MakernoteDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new NikonType1MakernoteDescriptor(this));
    }

    public override string Name => "Nikon Makernote";
}

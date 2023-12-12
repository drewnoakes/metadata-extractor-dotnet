// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Photoshop;

/// <summary>Holds the basic metadata found in the header of a Photoshop PSD file.</summary>
/// <author>Drew Noakes https://drewnoakes.com</author>
public class PsdHeaderDirectory : Directory
{
    /// <summary>The number of channels in the image, including any alpha channels.</summary>
    /// <remarks>Supported range is 1 to 56.</remarks>
    public const int TagChannelCount = 1;

    /// <summary>The height of the image in pixels.</summary>
    public const int TagImageHeight = 2;

    /// <summary>The width of the image in pixels.</summary>
    public const int TagImageWidth = 3;

    /// <summary>The number of bits per channel.</summary>
    /// <remarks>Supported values are 1, 8, 16 and 32.</remarks>
    public const int TagBitsPerChannel = 4;

    /// <summary>The color mode of the file.</summary>
    /// <remarks>
    /// Supported values are:
    /// Bitmap = 0; Grayscale = 1; Indexed = 2; RGB = 3; CMYK = 4; Multichannel = 7; Duotone = 8; Lab = 9.
    /// </remarks>
    public const int TagColorMode = 5;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagChannelCount, "Channel Count" },
        { TagImageHeight, "Image Height" },
        { TagImageWidth, "Image Width" },
        { TagBitsPerChannel, "Bits Per Channel" },
        { TagColorMode, "Color Mode" }
    };

    public PsdHeaderDirectory() : base(_tagNameMap)
    {
        SetDescriptor(new PsdHeaderDescriptor(this));
    }

    public override string Name => "PSD Header";
}

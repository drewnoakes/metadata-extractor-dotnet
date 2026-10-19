// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif;

/// <summary>One of several Exif directories.</summary>
/// <remarks>Otherwise known as IFD1, this directory holds information about an embedded thumbnail image.</remarks>
/// <author>Drew Noakes https://drewnoakes.com</author>
public sealed class ExifThumbnailDirectory : ExifDirectoryBase
{
    /// <summary>
    /// The offset to thumbnail image bytes, relative to the start of the IFD.
    /// </summary>
    /// <remarks>
    /// To obtain the offset relative to the start of the TIFF data stream, use the
    /// <see cref="AdjustedThumbnailOffset"/> property, which includes the value of
    /// <see cref="ExifStartOffset"/>.
    /// </remarks>
    public const int TagThumbnailOffset = 0x0201;

    /// <summary>The size of the thumbnail image data in bytes.</summary>
    public const int TagThumbnailLength = 0x0202;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagThumbnailOffset, "Thumbnail Offset" },
        { TagThumbnailLength, "Thumbnail Length" }
    };

    static ExifThumbnailDirectory()
    {
        AddExifTagNames(_tagNameMap);
    }

    public ExifThumbnailDirectory(int exifStartOffset) : base(_tagNameMap)
    {
        SetDescriptor(new ExifThumbnailDescriptor(this));

        ExifStartOffset = exifStartOffset;
    }

    public override string Name => "Exif Thumbnail";

    /// <summary>
    /// Gets the offset at which the Exif data stream commenced within any containing stream.
    /// </summary>
    public int ExifStartOffset { get; }

    /// <summary>
    /// Returns the offset to thumbnail data within the outermost data stream.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The value for <see cref="TagThumbnailOffset"/> is relative to the Exif data stream.
    /// Generally, consumers of thumbnail data need this value relative to the outermost stream,
    /// so that the thumbnail data may be extracted from that stream.
    /// </para>
    /// <para>
    /// This property adds the value of <see cref="ExifStartOffset"/> to this tag's value in order
    /// to produce that value.
    /// </para>
    /// <para>
    /// Returns <see langword="null"/> when the tag is not defined in this directory.
    /// </para>
    /// </remarks>
    public int? AdjustedThumbnailOffset => this.TryGetInt32(TagThumbnailOffset, out int offset) ? offset + ExifStartOffset : null;
}

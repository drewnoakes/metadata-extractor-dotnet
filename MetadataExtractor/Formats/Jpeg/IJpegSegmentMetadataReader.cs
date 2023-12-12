// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Jpeg;

/// <summary>
/// Defines an object that extracts metadata from JPEG segments.
/// </summary>
public interface IJpegSegmentMetadataReader
{
    /// <summary>
    /// Gets the set of JPEG segment types that this reader is interested in.
    /// </summary>
    ICollection<JpegSegmentType> SegmentTypes { get; }

    /// <summary>
    /// Extracts metadata from all JPEG segments matching <see cref="SegmentTypes"/>.
    /// </summary>
    /// <param name="segments">
    /// A sequence of JPEG segments from which the metadata should be extracted. These are in the order encountered in the original file.
    /// </param>
    IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments);
}

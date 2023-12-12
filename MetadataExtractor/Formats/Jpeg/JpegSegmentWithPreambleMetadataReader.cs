// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Jpeg;

public abstract class JpegSegmentWithPreambleMetadataReader : IJpegSegmentMetadataReader
{
    protected abstract byte[] PreambleBytes { get; }

    public abstract ICollection<JpegSegmentType> SegmentTypes { get; }

    public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
    {
        var preamble = PreambleBytes;

        // Skip segments not starting with the required preamble
        return segments
            .Where(segment => segment.Bytes.StartsWith(preamble))
            .SelectMany(segment => Extract(segment.Bytes, preambleLength: preamble.Length));
    }

    protected abstract IEnumerable<Directory> Extract(byte[] segmentBytes, int preambleLength);
}

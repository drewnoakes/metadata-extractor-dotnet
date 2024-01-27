// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Jpeg
{
    public abstract class JpegSegmentWithPreambleMetadataReader : IJpegSegmentMetadataReader
    {
        protected abstract ReadOnlySpan<byte> PreambleBytes { get; }

        public abstract IReadOnlyCollection<JpegSegmentType> SegmentTypes { get; }

        public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // Skip segments not starting with the required preamble
            return segments
                .Where(segment => segment.Span.StartsWith(PreambleBytes))
                .SelectMany(segment => Extract(segment.Bytes, preambleLength: PreambleBytes.Length));
        }

        protected abstract IEnumerable<Directory> Extract(byte[] segmentBytes, int preambleLength);
    }
}

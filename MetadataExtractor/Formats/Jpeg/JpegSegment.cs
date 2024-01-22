// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>
    /// Holds information about a JPEG segment.
    /// </summary>
    /// <seealso cref="JpegSegmentReader"/>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegSegment(JpegSegmentType type, byte[] bytes, long offset)
    {
        public JpegSegmentType Type { get; } = type;

        public byte[] Bytes { get; } = bytes;

        /// <summary>
        /// The start offset of the segment within the JPEG stream from which they were extracted.
        /// </summary>
        public long Offset { get; } = offset;

        internal ReadOnlySpan<byte> Span => Bytes.AsSpan();

        public override string ToString() => $"[{Type}] {Bytes.Length:N0} bytes at offset {Offset}";
    }
}

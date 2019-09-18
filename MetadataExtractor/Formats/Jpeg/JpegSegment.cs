// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>
    /// Holds information about a JPEG segment.
    /// </summary>
    /// <seealso cref="JpegSegmentReader"/>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegSegment
    {
        public JpegSegmentType Type { get; }
        public byte[] Bytes { get; }
        public long Offset { get; }

        public JpegSegment(JpegSegmentType type, byte[] bytes, long offset)
        {
            Type = type;
            Bytes = bytes;
            Offset = offset;
        }
    }
}

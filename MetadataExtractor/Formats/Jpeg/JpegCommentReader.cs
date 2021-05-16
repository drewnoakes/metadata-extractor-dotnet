// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Reads JPEG comments.</summary>
    /// <remarks>JPEG files can store zero or more comments in COM segments.</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegCommentReader : IJpegSegmentMetadataReader
    {
        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes { get; } = new[] { JpegSegmentType.Com };

        /// <summary>Reads JPEG comments, returning each in a <see cref="JpegCommentDirectory"/>.</summary>
        public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // The entire contents of the segment are the comment
            return segments.Select(segment => (Directory)new JpegCommentDirectory(new StringValue(segment.Bytes, Encoding.UTF8)));
        }
    }
}

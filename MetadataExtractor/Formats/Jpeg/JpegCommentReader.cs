// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Reads JPEG comments.</summary>
    /// <remarks>JPEG files can store zero or more comments in COM segments.</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegCommentReader : IJpegSegmentMetadataReader
    {
        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.Com };

        /// <summary>Reads JPEG comments, returning each in a <see cref="JpegCommentDirectory"/>.</summary>
        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // The entire contents of the segment are the comment
            return segments.Select(segment => new JpegCommentDirectory(new StringValue(segment.Bytes, Encoding.UTF8)))
#if NET35
                .Cast<Directory>()
#endif
                .ToList();
        }
    }
}

// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor.IO;
#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Decodes JPEG DNL image height data.</summary>
    /// <remarks>The current implementation only calls this reader if image height is missing from the JPEG SOFx segment.</remarks>
    /// <seealso cref="JpegSegment"/>
    /// <author>Nadahar</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class JpegDnlReader : IJpegSegmentMetadataReader
    {
        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new[] { JpegSegmentType.Dnl };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            return segments.Select(segment => Extract(new SequentialByteArrayReader(segment.Bytes)))
#if NET35
                .Cast<Directory>()
#endif
                .ToList();
        }

        public JpegDnlDirectory Extract(SequentialReader reader)
        {
            var directory = new JpegDnlDirectory();

            try
            {
                directory.Set(JpegDnlDirectory.TagImageHeight, reader.GetUInt16());
            }
            catch (IOException me)
            {
                directory.AddError(me.ToString());
            }

            return directory;
        }
    }
}

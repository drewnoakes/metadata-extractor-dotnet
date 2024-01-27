// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Decodes JPEG DNL image height data.</summary>
    /// <remarks>The current implementation only calls this reader if image height is missing from the JPEG SOFx segment.</remarks>
    /// <seealso cref="JpegSegment"/>
    /// <author>Nadahar</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class JpegDnlReader : IJpegSegmentMetadataReader
    {
        IReadOnlyCollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes { get; } = [JpegSegmentType.Dnl];

        public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            return segments.Select(segment => (Directory)Extract(new SequentialByteArrayReader(segment.Bytes)));
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

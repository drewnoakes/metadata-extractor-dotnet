// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Adobe
{
    /// <summary>Decodes Adobe formatted data stored in JPEG files, normally in the APPE (App14) segment.</summary>
    /// <author>Philip</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class AdobeJpegReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "Adobe";

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.AppE };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            return segments
                .Where(segment => segment.Bytes.Length == 12 && JpegSegmentPreamble.Equals(Encoding.UTF8.GetString(segment.Bytes, 0, JpegSegmentPreamble.Length), StringComparison.OrdinalIgnoreCase))
                .Select(bytes => Extract(new SequentialByteArrayReader(bytes.Bytes)))
#if NET35
                .Cast<Directory>()
#endif
                .ToList();
        }

        public AdobeJpegDirectory Extract(SequentialReader reader)
        {
            reader = reader.WithByteOrder(isMotorolaByteOrder: false);

            var directory = new AdobeJpegDirectory();

            try
            {
                if (reader.GetString(JpegSegmentPreamble.Length, Encoding.UTF8) != JpegSegmentPreamble)
                {
                    directory.AddError("Invalid Adobe JPEG data header.");
                    return directory;
                }

                directory.Set(AdobeJpegDirectory.TagDctEncodeVersion, reader.GetUInt16());
                directory.Set(AdobeJpegDirectory.TagApp14Flags0, reader.GetUInt16());
                directory.Set(AdobeJpegDirectory.TagApp14Flags1, reader.GetUInt16());
                directory.Set(AdobeJpegDirectory.TagColorTransform, reader.GetSByte());
            }
            catch (IOException ex)
            {
                directory.AddError("IO exception processing data: " + ex.Message);
            }

            return directory;
        }
    }
}

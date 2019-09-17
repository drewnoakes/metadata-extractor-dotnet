// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Reads Photoshop "ducky" segments, created during Save-for-Web.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class DuckyReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "Ducky";

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.AppC };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // Skip segments not starting with the required header
            return segments
                .Where(segment => segment.Bytes.Length >= JpegSegmentPreamble.Length && JpegSegmentPreamble == Encoding.UTF8.GetString(segment.Bytes, 0, JpegSegmentPreamble.Length))
                .Select(segment => Extract(new SequentialByteArrayReader(segment.Bytes, JpegSegmentPreamble.Length)))
#if NET35
                .Cast<Directory>()
#endif
                .ToList();
        }

        public DuckyDirectory Extract(SequentialReader reader)
        {
            var directory = new DuckyDirectory();

            try
            {
                while (true)
                {
                    var tag = reader.GetUInt16();

                    // End of Segment is marked with zero
                    if (tag == 0)
                        break;

                    var length = reader.GetUInt16();

                    switch (tag)
                    {
                        case DuckyDirectory.TagQuality:
                        {
                            if (length != 4)
                            {
                                directory.AddError("Unexpected length for the quality tag");
                                return directory;
                            }
                            directory.Set(tag, reader.GetUInt32());
                            break;
                        }
                        case DuckyDirectory.TagComment:
                        case DuckyDirectory.TagCopyright:
                        {
                            reader.Skip(4);
                            directory.Set(tag, reader.GetString(length - 4, Encoding.BigEndianUnicode));
                            break;
                        }
                        default:
                        {
                            // Unexpected tag
                            directory.Set(tag, reader.GetBytes(length));
                            break;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                directory.AddError(e.Message);
            }

            return directory;
        }
    }
}

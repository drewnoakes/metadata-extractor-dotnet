// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Reads Photoshop "ducky" segments, created during Save-for-Web.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class DuckyReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "Ducky";
        internal static ReadOnlySpan<byte> JpegSegmentPreambleBytes => "Ducky"u8;

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes { get; } = [JpegSegmentType.AppC];

        public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // Skip segments not starting with the required header
            return segments
                .Where(static segment => segment.Span.StartsWith(JpegSegmentPreambleBytes))
                .Select(segment => (Directory)Extract(new SequentialByteArrayReader(segment.Bytes, JpegSegmentPreamble.Length)));
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

                    int length = reader.GetUInt16();

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
                            length -= 4;
                            if (length < 0)
                            {
                                directory.AddError("Unexpected length for a text tag");
                                return directory;
                            }
                            directory.Set(tag, reader.GetString(length, Encoding.BigEndianUnicode));
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

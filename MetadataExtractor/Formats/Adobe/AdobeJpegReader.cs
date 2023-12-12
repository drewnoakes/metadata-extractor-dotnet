// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;

namespace MetadataExtractor.Formats.Adobe;

/// <summary>Decodes Adobe formatted data stored in JPEG files, normally in the APPE (App14) segment.</summary>
/// <author>Philip</author>
/// <author>Drew Noakes https://drewnoakes.com</author>
public sealed class AdobeJpegReader : IJpegSegmentMetadataReader
{
    public const string JpegSegmentPreamble = "Adobe";

    ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes { get; } = new[] { JpegSegmentType.AppE };

    public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
    {
        return segments
            .Where(segment => segment.Bytes.Length == 12 && JpegSegmentPreamble.Equals(Encoding.UTF8.GetString(segment.Bytes, 0, JpegSegmentPreamble.Length), StringComparison.OrdinalIgnoreCase))
            .Select(bytes => (Directory)Extract(new SequentialByteArrayReader(bytes.Bytes)));
    }

    public AdobeJpegDirectory Extract(SequentialReader reader)
    {
        reader = reader.WithByteOrder(isMotorolaByteOrder: false);

        var directory = new AdobeJpegDirectory();

        try
        {
            if (reader.GetString(JpegSegmentPreamble.Length, Encoding.ASCII) != JpegSegmentPreamble)
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

// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Tiff;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Decodes Exif binary data into potentially many <see cref="Directory"/> objects such as
    /// <see cref="ExifSubIfdDirectory"/>, <see cref="ExifThumbnailDirectory"/>, <see cref="ExifInteropDirectory"/>,
    /// <see cref="GpsDirectory"/>, camera makernote directories and more.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifReader : JpegSegmentWithPreambleMetadataReader
    {
        public static ReadOnlySpan<byte> JpegSegmentPreamble => "Exif\x0\x0"u8;

        public static bool StartsWithJpegExifPreamble(byte[] bytes) => bytes.AsSpan().StartsWith(JpegSegmentPreamble);

        public static int JpegSegmentPreambleLength => JpegSegmentPreamble.Length;

        /// <summary>Exif data stored in JPEG files' APP1 segment are preceded by this six character preamble "Exif\0\0".</summary>
        protected override ReadOnlySpan<byte> PreambleBytes => JpegSegmentPreamble;

        public override ICollection<JpegSegmentType> SegmentTypes { get; } = [JpegSegmentType.App1];

        protected override IEnumerable<Directory> Extract(byte[] segmentBytes, int preambleLength)
        {
            return Extract(new ByteArrayReader(segmentBytes, baseOffset: preambleLength), preambleLength);
        }

        /// <summary>
        /// Reads TIFF formatted Exif data a specified offset within a <see cref="IndexedReader"/>.
        /// </summary>
        public IReadOnlyList<Directory> Extract(IndexedReader reader, int exifStartOffset)
        {
            var directories = new List<Directory>();
            var exifTiffHandler = new ExifTiffHandler(directories, exifStartOffset);

            try
            {
                // Read the TIFF-formatted Exif data
                TiffReader.ProcessTiff(reader, exifTiffHandler);
            }
            catch (Exception e)
            {
                exifTiffHandler.Error("Exception processing TIFF data: " + e.Message);
            }

            return directories;
        }
    }
}

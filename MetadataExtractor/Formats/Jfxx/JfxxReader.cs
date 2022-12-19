// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;

namespace MetadataExtractor.Formats.Jfxx
{
    /// <summary>Reads JFXX (JFIF Extensions) data.</summary>
    /// <remarks>
    /// JFXX is found in JPEG APP0 segments.
    /// <list type="bullet">
    ///   <item>http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format</item>
    ///   <item>http://www.w3.org/Graphics/JPEG/jfif3.pdf</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes</author>
    public sealed class JfxxReader : JpegSegmentWithPreambleMetadataReader
    {
        public const string JpegSegmentPreamble = "JFXX";

        protected override byte[] PreambleBytes { get; } = Encoding.ASCII.GetBytes(JpegSegmentPreamble);

        public override ICollection<JpegSegmentType> SegmentTypes { get; } = new[] { JpegSegmentType.App0 };

        protected override IEnumerable<Directory> Extract(byte[] segmentBytes, int preambleLength)
        {
            yield return Extract(new ByteArrayReader(segmentBytes));
        }

        /// <summary>Reads JFXX values and returns them in an <see cref="JfxxDirectory"/>.</summary>
        public JfxxDirectory Extract(IndexedReader reader)
        {
            var directory = new JfxxDirectory();

            try
            {
                // For JFIF the tag number is the value's byte offset
                directory.Set(JfxxDirectory.TagExtensionCode, reader.GetByte(JfxxDirectory.TagExtensionCode));
            }
            catch (IOException e)
            {
                directory.AddError(e.Message);
            }

            return directory;
        }
    }
}

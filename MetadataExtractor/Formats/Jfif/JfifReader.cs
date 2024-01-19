// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;

namespace MetadataExtractor.Formats.Jfif
{
    /// <summary>Reads JFIF (JPEG File Interchange Format) data.</summary>
    /// <remarks>
    /// JFIF is found in JPEG APP0 segments.
    /// <list type="bullet">
    ///   <item>http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format</item>
    ///   <item>http://www.w3.org/Graphics/JPEG/jfif3.pdf</item>
    /// </list>
    /// </remarks>
    /// <author>Yuri Binev, Drew Noakes, Markus Meyer</author>
    public sealed class JfifReader : JpegSegmentWithPreambleMetadataReader
    {
        public const string JpegSegmentPreamble = "JFIF";

        protected override ReadOnlySpan<byte> PreambleBytes => "JFIF"u8;

        public override ICollection<JpegSegmentType> SegmentTypes { get; } = [JpegSegmentType.App0];

        protected override IEnumerable<Directory> Extract(byte[] segmentBytes, int preambleLength)
        {
            yield return Extract(new ByteArrayReader(segmentBytes));
        }

        /// <summary>Reads JFIF values and returns them in an <see cref="JfifDirectory"/>.</summary>
        public JfifDirectory Extract(IndexedReader reader)
        {
            var directory = new JfifDirectory();

            try
            {
                // For JFIF the tag number is the value's byte offset
#pragma warning disable format
                directory.Set(JfifDirectory.TagVersion,     reader.GetUInt16(JfifDirectory.TagVersion));
                directory.Set(JfifDirectory.TagUnits,       reader.GetByte(JfifDirectory.TagUnits));
                directory.Set(JfifDirectory.TagResX,        reader.GetUInt16(JfifDirectory.TagResX));
                directory.Set(JfifDirectory.TagResY,        reader.GetUInt16(JfifDirectory.TagResY));
                directory.Set(JfifDirectory.TagThumbWidth,  reader.GetByte(JfifDirectory.TagThumbWidth));
                directory.Set(JfifDirectory.TagThumbHeight, reader.GetByte(JfifDirectory.TagThumbHeight));
#pragma warning restore format
            }
            catch (IOException e)
            {
                directory.AddError(e.Message);
            }

            return directory;
        }
    }
}

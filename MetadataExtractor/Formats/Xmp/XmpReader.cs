// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;
using XmpCore;
using XmpCore.Options;

namespace MetadataExtractor.Formats.Xmp
{
    /// <summary>Extracts XMP data JPEG APP1 segments.</summary>
    /// <remarks>
    /// XMP uses a namespace and path format for identifying values, which does not map to metadata-extractor's
    /// integer based tag identifiers. Therefore, XMP data is extracted and exposed via <see cref="XmpDirectory.XmpMeta"/>
    /// which returns an instance of Adobe's <see cref="IXmpMeta"/> which exposes the full XMP data set.
    /// <para />
    /// The extraction is done with Adobe's XmpCore-Library (XMP-Toolkit)
    /// Copyright (c) 1999 - 2007, Adobe Systems Incorporated All rights reserved.
    /// </remarks>
    /// <author>Torsten Skadell</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "http://ns.adobe.com/xap/1.0/\0";
        public const string JpegSegmentPreambleExtension = "http://ns.adobe.com/xmp/extension/\0";

        private static byte[] JpegSegmentPreambleBytes { get; } = Encoding.UTF8.GetBytes(JpegSegmentPreamble);
        private static byte[] JpegSegmentPreambleExtensionBytes { get; } = Encoding.UTF8.GetBytes(JpegSegmentPreambleExtension);

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes { get; } = new[] { JpegSegmentType.App1 };

        public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // Ensure collection materialised (avoiding multiple lazy enumeration)
            segments = segments.ToList();

            foreach (var segment in segments)
            {
                if (IsXmpSegment(segment))
                {
                    yield return Extract(segment.Bytes, JpegSegmentPreambleBytes.Length, segment.Bytes.Length - JpegSegmentPreambleBytes.Length);
                }
            }

            var extensionGroups = segments.Where(IsExtendedXmpSegment).GroupBy(GetExtendedDataGuid);

            foreach (var extensionGroup in extensionGroups)
            {
                var buffer = new MemoryStream();
                foreach (var segment in extensionGroup)
                {
                    var n = JpegSegmentPreambleExtensionBytes.Length + 32 + 4 + 4;
                    buffer.Write(segment.Bytes, n, segment.Bytes.Length - n);
                }

                buffer.Position = 0;
                var directory = new XmpDirectory();
                var xmpMeta = XmpMetaFactory.Parse(buffer);
                directory.SetXmpMeta(xmpMeta);
                yield return directory;
            }
        }

        private static string GetExtendedDataGuid(JpegSegment segment) => Encoding.UTF8.GetString(segment.Bytes, JpegSegmentPreambleExtensionBytes.Length, 32);

        private static bool IsXmpSegment(JpegSegment segment) => segment.Bytes.StartsWith(JpegSegmentPreambleBytes);
        private static bool IsExtendedXmpSegment(JpegSegment segment) => segment.Bytes.StartsWith(JpegSegmentPreambleExtensionBytes);

        public XmpDirectory Extract(byte[] xmpBytes) => Extract(xmpBytes, 0, xmpBytes.Length);

        public XmpDirectory Extract(byte[] xmpBytes, int offset, int length)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Must be zero or greater.");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Must be zero or greater.");
            if (xmpBytes.Length < offset + length)
                throw new ArgumentException("Extends beyond length of byte array.", nameof(length));

            // Trim any trailing null bytes
            // https://github.com/drewnoakes/metadata-extractor-dotnet/issues/154
            while (xmpBytes[offset + length - 1] == 0)
                length--;

            var directory = new XmpDirectory();
            try
            {
                // Limit photoshop:DocumentAncestors node as it can reach over 100000 items and make parsing extremely slow. 
                // This is not a typical value but it may happen https://forums.adobe.com/thread/2081839
                var parseOptions = new ParseOptions();
                parseOptions.SetXMPNodesToLimit(new Dictionary<string, int>() { { "photoshop:DocumentAncestors", 1000 } });

                var xmpMeta = XmpMetaFactory.ParseFromBuffer(xmpBytes, offset, length, parseOptions);
                directory.SetXmpMeta(xmpMeta);
            }
            catch (XmpException e)
            {
                directory.AddError("Error processing XMP data: " + e.Message);
            }
            return directory;
        }
    }
}

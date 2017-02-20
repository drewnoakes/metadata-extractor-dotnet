#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

using System.Text;

using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Jfif;
using MetadataExtractor.Formats.Jfxx;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;
using MetadataExtractor.Util;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Parses the structure of JPEG data, returning contained segments.</summary>
    /// <remarks>
    /// JPEG files are composed of a sequence of consecutive JPEG segments. Each segment has a type <see cref="JpegSegmentType"/>.
    /// A JPEG file can contain multiple segments having the same type.
    /// <para />
    /// Segments are returned in the order they appear in the file, however that order may vary from file to file.
    /// <para />
    /// Use <see cref="ReadSegments(Stream,ICollection{JpegSegmentType})"/> to specific segment types,
    /// or pass <c>null</c> to read all segments.
    /// <para />
    /// Note that SOS (start of scan) or EOI (end of image) segments are not returned by this class's methods.
    /// </remarks>
    /// <seealso cref="JpegSegment"/>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class JpegSegmentReader
    {
        private static readonly ByteTrie<string> _appSegmentByPreambleBytes;

        static JpegSegmentReader()
        {
            _appSegmentByPreambleBytes = new ByteTrie<string>();

            _appSegmentByPreambleBytes.AddPath(AdobeJpegReader.JpegSegmentId, Encoding.UTF8.GetBytes(AdobeJpegReader.JpegSegmentPreamble));
            _appSegmentByPreambleBytes.AddPath(DuckyReader.JpegSegmentId, Encoding.UTF8.GetBytes(DuckyReader.JpegSegmentPreamble));
            _appSegmentByPreambleBytes.AddPath(ExifReader.JpegSegmentId, Encoding.UTF8.GetBytes(ExifReader.JpegSegmentPreamble));
            _appSegmentByPreambleBytes.AddPath(IccReader.JpegSegmentId, Encoding.UTF8.GetBytes(IccReader.JpegSegmentPreamble));
            _appSegmentByPreambleBytes.AddPath(JfifReader.JpegSegmentId, Encoding.UTF8.GetBytes(JfifReader.JpegSegmentPreamble));
            _appSegmentByPreambleBytes.AddPath(JfxxReader.JpegSegmentId, Encoding.UTF8.GetBytes(JfxxReader.JpegSegmentPreamble));
            _appSegmentByPreambleBytes.AddPath(PhotoshopReader.JpegSegmentId, Encoding.UTF8.GetBytes(PhotoshopReader.JpegSegmentPreamble));
            _appSegmentByPreambleBytes.AddPath(XmpReader.JpegSegmentId, Encoding.UTF8.GetBytes(XmpReader.JpegSegmentPreamble));
            _appSegmentByPreambleBytes.AddPath(XmpReader.JpegSegmentExtensionId, Encoding.UTF8.GetBytes(XmpReader.JpegSegmentPreambleExtension));
        }

        /// <summary>
        /// Walks the provided JPEG data, returning <see cref="JpegSegment"/> objects.
        /// </summary>
        /// <remarks>
        /// Will not return SOS (start of scan) or EOI (end of image) segments.
        /// </remarks>
        /// <param name="filePath">a file from which the JPEG data will be read.</param>
        /// <param name="segmentTypes">the set of JPEG segments types that are to be returned. If this argument is <c>null</c> then all found segment types are returned.</param>
        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="IOException"/>

        [NotNull]
        public static IEnumerable<JpegSegment> ReadSegments([NotNull] string filePath, [CanBeNull] ICollection<JpegSegmentType> segmentTypes = null)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return ReadSegments(stream, segmentTypes).ToList();
            }
        }

        /// <summary>
        /// Processes the provided JPEG data, and extracts the specified JPEG segments into a <see cref="JpegSegment"/> object.
        /// </summary>
        /// <remarks>
        /// Will not return SOS (start of scan) or EOI (end of image) segments.
        /// </remarks>
        /// <param name="stream">a <see cref="Stream"/> from which the JPEG data will be read. It must be positioned at the beginning of the JPEG data stream.</param>
        /// <param name="segmentTypes">the set of JPEG segments types that are to be returned. If this argument is <c>null</c> then all found segment types are returned.</param>
        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="IOException"/>
        [NotNull]
        public static IEnumerable<JpegSegment> ReadSegments([NotNull] Stream stream, [CanBeNull] ICollection<JpegSegmentType> segmentTypes = null)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Must be able to seek.", nameof(stream));

            // first two bytes should be JPEG magic number
            var magicNumber = GetUInt16(stream);

            if (magicNumber != 0xFFD8)
                throw new JpegProcessingException($"JPEG data is expected to begin with 0xFFD8 (ÿØ) not 0x{magicNumber:X4}");

            while (true)
            {
                var padding = 0;

                // Find the segment marker. Markers are zero or more 0xFF bytes, followed
                // by a 0xFF and then a byte not equal to 0x00 or 0xFF.
                var segmentIdentifier = stream.ReadByte();
                var segmentTypeByte = stream.ReadByte();

                if (segmentTypeByte == -1)
                    yield break;

                // Read until we have a 0xFF byte followed by a byte that is not 0xFF or 0x00
                while (segmentIdentifier != 0xFF || segmentTypeByte == 0xFF || segmentTypeByte == 0)
                {
                    padding++;
                    segmentIdentifier = segmentTypeByte;
                    segmentTypeByte = stream.ReadByte();

                    if (segmentTypeByte == -1)
                        yield break;
                }

                var segmentType = (JpegSegmentType)segmentTypeByte;
                var offset = stream.Position;

                // if there is a payload, then segment length includes the two size bytes
                if (segmentType.ContainsPayload())
                {
                    //var pos = stream.Position;

                    // Read the 2-byte big-endian segment length (excludes two marker bytes)
                    var b1 = stream.ReadByte();
                    var b2 = stream.ReadByte();
                    if (b2 == -1)
                        yield break;
                    var segmentLength = GetUInt16(b1, b2); // unchecked((ushort)(b1 << 8 | b2));

                    // A length of less than two would be an error
                    if (segmentLength < 2)
                        yield break;

                    // get position after id and type bytes (beginning of payload)
                    offset += 2;
                    //offset = stream.Position;

                    // TODO: would you rather break here or throw an exception?
                    if (segmentLength > (stream.Length - offset + 1))
                        yield break;
                    //    throw new JpegProcessingException($"Segment {segmentType} is truncated. Processing cannot proceed.");

                    // segment length includes size bytes, so subtract two
                    segmentLength -= 2;

                    // Check whether we are interested in this segment
                    if (segmentTypes == null || segmentTypes.Contains(segmentType))
                    {
                        var preambleBytes = new byte[Math.Min(segmentLength, _appSegmentByPreambleBytes.MaxDepth)];
                        if (stream.Read(preambleBytes, 0, preambleBytes.Length) != preambleBytes.Length)
                            yield break;
                        var preamble = _appSegmentByPreambleBytes.Find(preambleBytes); //?? "";

                        // Preamble wasn't found in the list. Since preamble is used in string comparisons, set it to blank.
                        // (TODO: this might be a good place to record unknown preambles and report back somehow)
                        if (preamble == null)
                            preamble = "";

                        yield return new JpegSegment(segmentType, segmentLength, padding, offset, preamble);
                    }

                    // seek to the end of the segment
                    stream.Position = offset + segmentLength;

                    // Sos means entropy encoded data immediately follows, ending with Eoi or another indicator
                    if (segmentType == JpegSegmentType.Sos)
                    {
                        //yield break;

                        // This is the actual encoded data for this scan.
                        // NOTE: a 0x00 byte that follows a 0xFF is a 'stuff byte.' This functions as an escape sequence.
                        //       The 0xFF should be seen as part of the data and not a new segment indicator.
                        //       TODO: If we decide to hold an encoding segment, any parsing directories should throw 
                        //       out the 0x00 bytes when decoding the actual image data.
                        while (true)
                        {
                            var next = stream.ReadByte();
                            if (next == -1)
                                yield break;
                            if (next == 0xFF)
                            {
                                next = stream.ReadByte();
                                if (next == -1)
                                    yield break;

                                // Ignore unless it's not 0x00 (stuff byte)
                                if (next != 0x00)
                                {
                                    // encountered a 0xFF followed by something other a 0x00.
                                    // Is likely a new segment, so backup two bytes and let the main while loop continue
                                    stream.Seek(-2, SeekOrigin.Current);
                                    break;
                                }
                            }
                        }
                        
                    }
                }
                else
                {
                    // Check whether we are interested in this non-payload segment
                    if (segmentTypes == null || segmentTypes.Contains(segmentType))
                        yield return new JpegSegment(segmentType, 0, padding, offset, "");

                    if (segmentType == JpegSegmentType.Eoi)
                        break;
                }

            }
        }

        private static ushort GetUInt16(Stream stream)
        {
            var b1 = stream.ReadByte();
            var b2 = stream.ReadByte();
            if (b2 == -1)
                throw new IOException("Unexpected end of stream.");
            return GetUInt16(b1, b2);
        }

        private static ushort GetUInt16(int b1, int b2)
        {
            return unchecked((ushort)(b1 << 8 | b2));
        }
    }
}

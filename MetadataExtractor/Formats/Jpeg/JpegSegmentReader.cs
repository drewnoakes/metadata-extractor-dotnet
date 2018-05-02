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
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Iptc;
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
    /// Use <see cref="ReadSegments(ReaderInfo,ICollection{JpegSegmentType})"/> to specific segment types,
    /// or pass <c>null</c> to read all segments.
    /// <para />
    /// Note that SOS (start of scan) or EOI (end of image) segments are not returned by this class's methods.
    /// </remarks>
    /// <seealso cref="JpegSegment"/>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class JpegSegmentReader
    {
        private static readonly ByteTrie<string> _appSegmentByPreambleBytes = new ByteTrie<string>
        {
            { AdobeJpegReader.JpegSegmentId,      Encoding.UTF8.GetBytes(AdobeJpegReader.JpegSegmentPreamble) },
            { DuckyReader.JpegSegmentId,          Encoding.UTF8.GetBytes(DuckyReader.JpegSegmentPreamble) },
            { ExifReader.JpegSegmentId,           Encoding.UTF8.GetBytes(ExifReader.JpegSegmentPreamble) },
            { IccReader.JpegSegmentId,            Encoding.UTF8.GetBytes(IccReader.JpegSegmentPreamble) },
            { JfifReader.JpegSegmentId,           Encoding.UTF8.GetBytes(JfifReader.JpegSegmentPreamble) },
            { JfxxReader.JpegSegmentId,           Encoding.UTF8.GetBytes(JfxxReader.JpegSegmentPreamble) },
            { PhotoshopReader.JpegSegmentId,      Encoding.UTF8.GetBytes(PhotoshopReader.JpegSegmentPreamble) },
            { XmpReader.JpegSegmentId,            Encoding.UTF8.GetBytes(XmpReader.JpegSegmentPreamble) },
            { XmpReader.JpegSegmentExtensionId,   Encoding.UTF8.GetBytes(XmpReader.JpegSegmentPreambleExtension) }
        };

        private static readonly HashSet<byte> _segmentMarkerBytes = new HashSet<byte>
        {
            IptcReader.IptcMarkerByte
        };        

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
                return ReadSegments(new RandomAccessStream(stream).CreateReader(), segmentTypes).ToList();
            /*
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                return ReadSegments(new SequentialStreamReader(stream), segmentTypes).ToList();
            */
        }

        /// <summary>
        /// Walks the provided JPEG data, returning <see cref="JpegSegment"/> objects.
        /// </summary>
        /// <param name="stream">a <see cref="Stream"/> from which the JPEG data will be read.</param>
        /// <param name="segmentTypes">the set of JPEG segments types that are to be returned. If this argument is <c>null</c> then all found segment types are returned.</param>
        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="IOException"/>
        public static IEnumerable<JpegSegment> ReadSegments([NotNull] Stream stream, [CanBeNull] ICollection<JpegSegmentType> segmentTypes = null)
        {
            return ReadSegments(new RandomAccessStream(stream).CreateReader(), segmentTypes);
        }

        public static IEnumerable<JpegSegment> ReadSegments([NotNull] byte[] bytes, [CanBeNull] ICollection<JpegSegmentType> segmentTypes = null)
        {
            return ReadSegments(new RandomAccessStream(bytes).CreateReader(), segmentTypes);
        }

        /// <summary>
        /// Processes the provided JPEG data, and extracts the specified JPEG segments into a <see cref="JpegSegment"/> object.
        /// </summary>
        /// <remarks>
        /// Will not return SOS (start of scan) or EOI (end of image) segments.
        /// </remarks>
        /// <param name="reader">a <see cref="ReaderInfo"/> from which the JPEG data will be read. It must be positioned at the beginning of the JPEG data stream.</param>
        /// <param name="segmentTypes">the set of JPEG segments types that are to be returned. If this argument is <c>null</c> then all found segment types are returned.</param>
        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="IOException"/>
        [NotNull]
        public static IEnumerable<JpegSegment> ReadSegments([NotNull] ReaderInfo reader, [CanBeNull] ICollection<JpegSegmentType> segmentTypes = null)
        {
            if (!reader.IsMotorolaByteOrder)
                throw new JpegProcessingException("Must be big-endian/Motorola byte order.");

            // first two bytes should be JPEG magic number
            var magicNumber = reader.GetUInt16();

            if (magicNumber != 0xFFD8)
                throw new JpegProcessingException($"JPEG data is expected to begin with 0xFFD8 (ÿØ) not 0x{magicNumber:X4}");

            while (true)
            {
                var padding = 0;

                // Find the segment marker. Markers are zero or more 0xFF bytes, followed
                // by a 0xFF and then a byte not equal to 0x00 or 0xFF.
                var segmentIdentifier = reader.GetByte();
                var segmentTypeByte = reader.GetByte();

                if (unchecked((sbyte)segmentTypeByte) == -1)
                    yield break;

                // Read until we have a 0xFF byte followed by a byte that is not 0xFF or 0x00
                while (segmentIdentifier != 0xFF || segmentTypeByte == 0xFF || segmentTypeByte == 0)
                {
                    padding++;
                    segmentIdentifier = segmentTypeByte;
                    segmentTypeByte = reader.GetByte();

                    if (unchecked((sbyte)segmentTypeByte) == -1)
                        yield break;
                }

                var segmentType = (JpegSegmentType)segmentTypeByte;

                // decide whether this JPEG segment type's marker is followed by a length indicator
                if (segmentType.ContainsPayload())
                {
                    /*var b1 = reader.GetByte();
                    var b2 = reader.GetByte();
                    if (unchecked((sbyte)b2) == -1)
                        yield break;

                    var segmentLength = reader.GetUInt16(b1, b2);*/

                    // Need two more bytes for the segment length. If closer than two bytes to the end, yield
                    if (reader.IsCloserToEnd(2))
                        yield break;

                    // Read the 2-byte big-endian segment length
                    // The length includes the two bytes for the length, but not the two bytes for the marker
                    var segmentLength = reader.GetUInt16();
                    
                    // A length of less than two would be an error
                    if (segmentLength < 2)
                        yield break;

                    // get position after id and type bytes (beginning of payload)
                    //offset += 2;

                    // TODO: would you rather break here or throw an exception?
                    /*if (segmentLength > (reader.Length - offset + 1))
                        yield break;        // throw new JpegProcessingException($"Segment {segmentType} is truncated. Processing cannot proceed.");
                    else
                    {*/
                    // segment length includes size bytes, so subtract two
                    segmentLength -= 2;
                    //}

                    // Check whether we are interested in this segment
                    if (segmentTypes == null || segmentTypes.Contains(segmentType))
                    {
                        var preambleBytes = new byte[Math.Min(segmentLength, _appSegmentByPreambleBytes.MaxDepth)];
                        var read = reader.Read(preambleBytes, 0, preambleBytes.Length);
                        if (read != preambleBytes.Length)
                            yield break;
                        var preamble = _appSegmentByPreambleBytes.Find(preambleBytes); //?? "";

                        // Preamble wasn't found in the list. Since preamble is used in string comparisons, set it to blank.
                        // (TODO: this might be a good place to record unknown preambles and report back somehow)
                        if (preamble == null)
                            preamble = "";

                        reader.Seek(-read);

                        byte bytemarker = 0x00;
                        if(preamble.Length == 0)
                        {
                            bytemarker = reader.GetByte();
                            if (!_segmentMarkerBytes.Contains(bytemarker))
                                bytemarker = 0x00;
                            reader.Seek(-1);
                        }
                        yield return new JpegSegment(segmentType, reader.Clone(segmentLength), preamble, bytemarker);
                    }

                    // seek to the end of the segment
                    reader.Seek(segmentLength);

                    // Sos means entropy encoded data immediately follows, ending with Eoi or another indicator
                    // We already did a seek to the end of the SOS segment. A byte-by-byte scan follows to find the next indicator
                    if (segmentType == JpegSegmentType.Sos)
                    {
                        // yielding here makes Sos processing work the old way (ending at the first one)
                        yield break;
                    }
                }
                else
                {
                    // Check whether we are interested in this non-payload segment
                    if (segmentTypes == null || segmentTypes.Contains(segmentType))
                        yield return new JpegSegment(segmentType, reader.Clone(0));

                    if (segmentType == JpegSegmentType.Eoi)
                        break;
                }

                //yield break;    // temp
            }
        }
    }
}

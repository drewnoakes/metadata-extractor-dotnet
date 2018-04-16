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
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Provides methods for reading and writing JPEG data without affecting other content.</summary>
    /// <remarks>
    /// JPEG files are composed of a sequence of consecutive JPEG segments, but the <see cref="JpegSegmentReader"/>
    /// does not read all bytes.  To edit JPEG data without affecting content other than the edited
    /// JpegSegment, reading + writing must consider non-segment-coding bytes too.
    /// </remarks>
    /// <seealso cref="JpegFragment"/>
    /// <author>Michael Osthege</author>
    public static class JpegFragmentWriter
    {
        /// <summary>
        /// Reads all bytes from a stream of JPEG data and splits it into JpegFragments.
        /// </summary>
        /// <param name="reader">
        /// A <see cref="SequentialReader"/> from which the JPEG data will be read.
        /// It must be positioned at the beginning of the JPEG data stream.
        /// </param>
        /// <returns>A list of all fragments that, when concatenated, match the input stream.</returns>
        [NotNull]
        public static List<JpegFragment> SplitFragments([NotNull] SequentialReader reader)
        {
            if (!reader.IsMotorolaByteOrder)
                throw new JpegProcessingException("Must be big-endian/Motorola byte order.");

            List<JpegFragment> fragments = new List<JpegFragment>();

            MemoryStream buffer = new MemoryStream();

            try
            {
                // The following loop walks over the bytes of the input
                // It accumulates all bytes in a buffer.
                // When a JpegSegment marker is identified, all (non-coding) bytes up to the marker
                // are made into a JpegFragment and collected.
                // The marker and its segment payload are also collected as a JpegFragment.
                // When the end of the input is reached, all remaining (non-coding) bytes are also
                // collected as one JpegFragment.

                while (true)
                {
                    // Read bytes into the buffer
                    // Find the segment marker. Markers are zero or more 0xFF bytes, followed
                    // by a 0xFF and then a byte not equal to 0x00 or 0xFF.
                    var segmentIdentifier = reader.GetByte();
                    buffer.WriteByte(segmentIdentifier);
                    var segmentTypeByte = reader.GetByte();
                    buffer.WriteByte(segmentTypeByte);

                    // Read until we have a 0xFF byte followed by a byte that is not 0xFF or 0x00
                    while (segmentIdentifier != 0xFF || segmentTypeByte == 0xFF || segmentTypeByte == 0)
                    {
                        segmentIdentifier = segmentTypeByte;
                        segmentTypeByte = reader.GetByte();
                        buffer.WriteByte(segmentTypeByte);
                    }
                    var segmentType = (JpegSegmentType)segmentTypeByte;

                    // The algorithm above stopped right after a segment marker.
                    // 1. All buffer up to the segment marker must become a fragment
                    if (buffer.Length > 2)
                    {
                        byte[] fragmentA = new byte[buffer.Length - 2];
                        buffer.Read(fragmentA, 0, fragmentA.Length);
                        fragments.Add(new JpegFragment(fragmentA));
                    }

                    // 2. The segment marker + the segment payload becomes a fragment
                    // To read the segment payload, the segment length must be decoded:
                    // next 2-bytes are <segment-size>: [high-byte] [low-byte]
                    var segmentLength = (int)reader.GetUInt16();
                    // segment length includes size bytes, so subtract two
                    segmentLength -= 2;
                    if (segmentLength < 0)
                        throw new JpegProcessingException("JPEG segment size would be less than zero.");
                    var segmentOffset = reader.Position;
                    var payloadBytes = reader.GetBytes(segmentLength);
                    Debug.Assert(segmentLength == payloadBytes.Length);
                    var segment = new JpegSegment(segmentType, payloadBytes, segmentOffset);
                    fragments.Add(JpegFragment.FromJpegSegment(segment, reader.IsMotorolaByteOrder));

                    // All buffer contents have been copied into fragments
                    buffer = new MemoryStream();
                }
            }
            catch (IOException ex)
            {
                // we expect a IOException when the sequential reader reaches the end of the original data
                if (ex.Message != "End of data reached.")
                    throw new JpegProcessingException("An error occured while trying to write Xml to the buffer.", ex);

                // The remaining buffer must also be collected.
                if (buffer.Length > 0)
                {
                    fragments.Add(new JpegFragment(buffer.ToArray()));
                }
            }
            catch (Exception ex)
            {
                throw new JpegProcessingException("An error occured while trying to write Xml to the buffer.", ex);
            }

            return fragments;
        }


        /// <summary>
        /// Concatenates the provided JpegFragments into a <see cref="MemoryStream"/> object.
        /// </summary>
        /// <param name="fragments">a list of JpegFragments that shall be joined.</param>
        [NotNull]
        public static MemoryStream JoinFragments([NotNull] IEnumerable<JpegFragment> fragments)
        {
            MemoryStream output = new MemoryStream();

            foreach (JpegFragment fragment in fragments)
            {
                output.Write(fragment.Bytes, 0, fragment.Bytes.Length);
            }

            return output;
        }

        /// <summary>
        /// Validates that the JpegFragments make up valid JPEG data.
        /// </summary>
        /// <param name="fragments">Ordered list of JpegFragments</param>
        /// <returns>true if passed</returns>
        public static bool IsValid([NotNull] IEnumerable<JpegFragment> fragments)
        {
            throw new NotImplementedException();
        }
    }
}

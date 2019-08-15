#region License
//
// Copyright 2002-2019 Drew Noakes
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

using JetBrains.Annotations;
using System;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>
    /// Holds information about a JPEG segment.
    /// </summary>
    /// <seealso cref="JpegSegmentReader"/>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegSegment
    {
        public JpegSegmentType Type { get; }
        [NotNull] public byte[] Bytes { get; }
        public long Offset { get; }

        public JpegSegment(JpegSegmentType type, [NotNull] byte[] bytes, long offset)
        {
            Type = type;
            Bytes = bytes;
            Offset = offset;
        }

        /// <summary>
        /// Computes the length of a segment payload from the high/low bytes of the index.
        /// (Segment length excludes the index bytes.)
        /// </summary>
        /// <param name="highByte">first byte of the index</param>
        /// <param name="lowByte">second byte of the index</param>
        /// <returns></returns>
        public static int DecodePayloadLength(byte highByte, byte lowByte)
        {
            // the segment length includes size bytes, so subtract two
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt16(new byte[] { lowByte, highByte }, 0) - 2;
            else
                return BitConverter.ToUInt16(new byte[] { highByte, lowByte }, 0) - 2;
        }

        /// <summary>
        /// Encodes (big endian) the length of a segment into the index bytes of the segment.
        /// </summary>
        /// <param name="payloadLength">Length of the payload (excludes the index)</param>
        /// <returns>segment-index bytes (length 2)</returns>
        public static byte[] EncodePayloadLength(int payloadLength)
        {
            // the segment length includes the high & low bytes, so add 2
            byte[] bytes = BitConverter.GetBytes(payloadLength + 2);
            if (BitConverter.IsLittleEndian)
                return new byte[] { bytes[1], bytes[0] };
            else
                return bytes;
        }
    }
}

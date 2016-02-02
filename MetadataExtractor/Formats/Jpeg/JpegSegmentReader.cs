﻿#region License
//
// Copyright 2002-2015 Drew Noakes
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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Performs read functions of JPEG files, returning specific file segments.</summary>
    /// <remarks>
    /// JPEG files are composed of a sequence of consecutive JPEG 'segments'. Each is identified by one of a set of byte
    /// values, modelled in the <see cref="JpegSegmentType"/> enumeration. Use <see cref="ReadSegments(SequentialReader,ICollection{JpegSegmentType})"/>
    /// to read out the some or all segments into a <see cref="JpegSegmentData"/> object, from which the raw JPEG segment byte arrays may be accessed.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class JpegSegmentReader
    {
#if !PORTABLE
        /// <summary>
        /// Processes the provided JPEG data, and extracts the specified JPEG segments into a <see cref="JpegSegmentData"/> object.
        /// </summary>
        /// <remarks>
        /// Will not return SOS (start of scan) or EOI (end of image) segments.
        /// </remarks>
        /// <param name="filePath">a file from which the JPEG data will be read.</param>
        /// <param name="segmentTypes">the set of JPEG segments types that are to be returned. If this argument is <c>null</c> then all found segment types are returned.</param>
        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static JpegSegmentData ReadSegments([NotNull] string filePath, [CanBeNull] ICollection<JpegSegmentType> segmentTypes)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                return ReadSegments(new SequentialStreamReader(stream), segmentTypes);
        }
#endif

        /// <summary>
        /// Processes the provided JPEG data, and extracts the specified JPEG segments into a <see cref="JpegSegmentData"/> object.
        /// </summary>
        /// <remarks>
        /// Will not return SOS (start of scan) or EOI (end of image) segments.
        /// </remarks>
        /// <param name="reader">a <see cref="SequentialReader"/> from which the JPEG data will be read. It must be positioned at the beginning of the JPEG data stream.</param>
        /// <param name="segmentTypes">the set of JPEG segments types that are to be returned. If this argument is <c>null</c> then all found segment types are returned.</param>
        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static JpegSegmentData ReadSegments([NotNull] SequentialReader reader, [CanBeNull] ICollection<JpegSegmentType> segmentTypes = null)
        {
            // Must be big-endian
            Debug.Assert(reader.IsMotorolaByteOrder);

            // first two bytes should be JPEG magic number
            var magicNumber = reader.GetUInt16();

            if (magicNumber != 0xFFD8)
                throw new JpegProcessingException($"JPEG data is expected to begin with 0xFFD8 (ÿØ) not 0x{magicNumber:X4}");

            var segmentData = new JpegSegmentData();
            do
            {
                // Find the segment marker. Markers are zero or more 0xFF bytes, followed
                // by a 0xFF and then a byte not equal to 0x00 or 0xFF.
                var segmentIdentifier = reader.GetByte();

                // We must have at least one 0xFF byte
                if (segmentIdentifier != 0xFF)
                    throw new JpegProcessingException($"Expected JPEG segment start identifier 0xFF, not 0x{segmentIdentifier:X2}");

                // Read until we have a non-0xFF byte. This identifies the segment type.
                var segmentTypeByte = reader.GetByte();
                while (segmentTypeByte == 0xFF)
                    segmentTypeByte = reader.GetByte();

                if (segmentTypeByte == 0)
                    throw new JpegProcessingException("Expected non-zero byte as part of JPEG marker identifier");

                var segmentType = (JpegSegmentType)segmentTypeByte;

                if (segmentType == JpegSegmentType.Sos)
                {
                    // The 'Start-Of-Scan' segment's length doesn't include the image data, instead would
                    // have to search for the two bytes: 0xFF 0xD9 (EOI).
                    // It comes last so simply return at this point
                    return segmentData;
                }

                if (segmentType == JpegSegmentType.Eoi)
                {
                    // the 'End-Of-Image' segment -- this should never be found in this fashion
                    return segmentData;
                }

                // next 2-bytes are <segment-size>: [high-byte] [low-byte]
                var segmentLength = (int)reader.GetUInt16();

                // segment length includes size bytes, so subtract two
                segmentLength -= 2;

                if (segmentLength < 0)
                    throw new JpegProcessingException("JPEG segment size would be less than zero");

                // Check whether we are interested in this segment
                if (segmentTypes == null || segmentTypes.Contains(segmentType))
                {
                    long segmentPosition = reader.Position;
                    var segmentBytes = reader.GetBytes(segmentLength);
                    Debug.Assert(segmentLength == segmentBytes.Length);
                    JpegSegment segment = new JpegSegment(segmentType, segmentBytes, segmentPosition);
                    segmentData.AddSegment(segmentType, segment);
                }
                else
                {
                    // Some of the JPEG is truncated, so just return what data we've already gathered
                    if (!reader.TrySkip(segmentLength))
                    {
                        return segmentData;
                    }
                }
            }
            while (true);
        }
    }
}

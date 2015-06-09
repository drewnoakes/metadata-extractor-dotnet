/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.Collections.Generic;
using System.Diagnostics;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Jpeg
{
    /// <summary>Performs read functions of JPEG files, returning specific file segments.</summary>
    /// <remarks>
    /// Performs read functions of JPEG files, returning specific file segments.
    /// <para>
    /// JPEG files are composed of a sequence of consecutive JPEG 'segments'. Each is identified by one of a set of byte
    /// values, modelled in the
    /// <see cref="JpegSegmentType"/>
    /// enumeration. Use <c>readSegments</c> to read out the some
    /// or all segments into a
    /// <see cref="JpegSegmentData"/>
    /// object, from which the raw JPEG segment byte arrays may be accessed.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class JpegSegmentReader
    {
        /// <summary>Private, because this segment crashes my algorithm, and searching for it doesn't work (yet).</summary>
        private const byte SegmentSos = unchecked((byte)0xDA);

        /// <summary>Private, because one wouldn't search for it.</summary>
        private const byte MarkerEoi = unchecked((byte)0xD9);

        /// <summary>
        /// Processes the provided JPEG data, and extracts the specified JPEG segments into a
        /// <see cref="JpegSegmentData"/>
        /// object.
        /// <para>
        /// Will not return SOS (start of scan) or EOI (end of image) segments.
        /// </summary>
        /// <param name="file">
        /// a
        /// <see cref="Sharpen.FilePath"/>
        /// from which the JPEG data will be read.
        /// </param>
        /// <param name="segmentTypes">
        /// the set of JPEG segments types that are to be returned. If this argument is <c>null</c>
        /// then all found segment types are returned.
        /// </param>
        /// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static JpegSegmentData ReadSegments([NotNull] FilePath file, [CanBeNull] IEnumerable<JpegSegmentType> segmentTypes)
        {
            FileInputStream stream = null;
            try
            {
                stream = new FileInputStream(file);
                return ReadSegments(new SequentialStreamReader(stream), segmentTypes);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// Processes the provided JPEG data, and extracts the specified JPEG segments into a
        /// <see cref="JpegSegmentData"/>
        /// object.
        /// <para>
        /// Will not return SOS (start of scan) or EOI (end of image) segments.
        /// </summary>
        /// <param name="reader">
        /// a
        /// <see cref="Com.Drew.Lang.SequentialReader"/>
        /// from which the JPEG data will be read. It must be positioned at the
        /// beginning of the JPEG data stream.
        /// </param>
        /// <param name="segmentTypes">
        /// the set of JPEG segments types that are to be returned. If this argument is <c>null</c>
        /// then all found segment types are returned.
        /// </param>
        /// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static JpegSegmentData ReadSegments([NotNull] SequentialReader reader, [CanBeNull] IEnumerable<JpegSegmentType> segmentTypes)
        {
            // Must be big-endian
            Debug.Assert((reader.IsMotorolaByteOrder()));
            // first two bytes should be JPEG magic number
            int magicNumber = reader.GetUInt16();
            if (magicNumber != unchecked(0xFFD8))
            {
                throw new JpegProcessingException("JPEG data is expected to begin with 0xFFD8 (ÿØ) not 0x" + Extensions.ToHexString(magicNumber));
            }
            ICollection<byte> segmentTypeBytes = null;
            if (segmentTypes != null)
            {
                segmentTypeBytes = new HashSet<byte>();
                foreach (JpegSegmentType segmentType in segmentTypes)
                {
                    segmentTypeBytes.Add(segmentType.ByteValue);
                }
            }
            JpegSegmentData segmentData = new JpegSegmentData();
            do
            {
                // Find the segment marker. Markers are zero or more 0xFF bytes, followed
                // by a 0xFF and then a byte not equal to 0x00 or 0xFF.
                short segmentIdentifier = reader.GetUInt8();
                // We must have at least one 0xFF byte
                if (segmentIdentifier != unchecked(0xFF))
                {
                    throw new JpegProcessingException("Expected JPEG segment start identifier 0xFF, not 0x" + Extensions.ToHexString(segmentIdentifier).ToUpper());
                }
                // Read until we have a non-0xFF byte. This identifies the segment type.
                byte segmentType = reader.GetInt8();
                while (segmentType == unchecked((byte)0xFF))
                {
                    segmentType = reader.GetInt8();
                }
                if (segmentType == 0)
                {
                    throw new JpegProcessingException("Expected non-zero byte as part of JPEG marker identifier");
                }
                if (segmentType == SegmentSos)
                {
                    // The 'Start-Of-Scan' segment's length doesn't include the image data, instead would
                    // have to search for the two bytes: 0xFF 0xD9 (EOI).
                    // It comes last so simply return at this point
                    return segmentData;
                }
                if (segmentType == MarkerEoi)
                {
                    // the 'End-Of-Image' segment -- this should never be found in this fashion
                    return segmentData;
                }
                // next 2-bytes are <segment-size>: [high-byte] [low-byte]
                int segmentLength = reader.GetUInt16();
                // segment length includes size bytes, so subtract two
                segmentLength -= 2;
                if (segmentLength < 0)
                {
                    throw new JpegProcessingException("JPEG segment size would be less than zero");
                }
                // Check whether we are interested in this segment
                if (segmentTypeBytes == null || segmentTypeBytes.Contains(segmentType))
                {
                    byte[] segmentBytes = reader.GetBytes(segmentLength);
                    Debug.Assert((segmentLength == segmentBytes.Length));
                    segmentData.AddSegment(segmentType, segmentBytes);
                }
                else
                {
                    // Some if the JPEG is truncated, just return what data we've already gathered
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

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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Jfif;
using MetadataExtractor.Formats.Jfxx;
using MetadataExtractor.Formats.Photoshop;
#if !PORTABLE
using MetadataExtractor.Formats.FileSystem;
#endif
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;
using System.Xml.Linq;
using System.Text;
using System;
using System.Xml;
using System.Diagnostics;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Writes metadata to JPEG formatted files.</summary>
    /// <author>Michael Osthege</author>
    public static class JpegMetadataSubstitutor
    {

        /// <summary>
        /// Writes a Xmp XDocument to a MemoryStream, starting with the buffer of an existing file.
        /// If no App1 segment is found, a new App1 segment will be inserted.
        /// </summary>
        /// <param name="original">Buffer of a file</param>
        /// <param name="xmp">XDocument to be written</param>
        /// <exception cref="JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static MemoryStream SubstituteXmp([NotNull] byte[] original, [NotNull] XDocument xmp)
        {
            SequentialByteArrayReader input = new SequentialByteArrayReader(original);
            MemoryStream output = new MemoryStream();

            try
            {
                // The Xmp-substitution loops over the original bytes and copies them into a
                // MemoryStream (ms).  If an App1-segment containing Xmp data is encountered,
                // the new Xmp bytes are written to the MemoryStream instead of the old bytes.
                // If no Xmp-containing App1 is encountered before other segments, a new App1-
                // segment with the Xmp-data is inserted.
                // All non-App1-fragments are just copied as they are.

                bool wroteXmp = false;
                // The following loop scans over the input, copies it to the output and locks in
                // when it encounters a segment marker.
                // Reference: http://dev.exiv2.org/projects/exiv2/wiki/The_Metadata_in_JPEG_files
                while (true)
                {
                    // Find the segment marker. Markers are zero or more 0xFF bytes, followed
                    // by a 0xFF and then a byte not equal to 0x00 or 0xFF.
                    var segmentIdentifier = input.GetByte();
                    output.WriteByte(segmentIdentifier);
                    var segmentTypeByte = input.GetByte();
                    output.WriteByte(segmentTypeByte);

                    // Read until we have a 0xFF byte followed by a byte that is not 0xFF or 0x00
                    while (segmentIdentifier != 0xFF || segmentTypeByte == 0xFF || segmentTypeByte == 0)
                    {
                        segmentIdentifier = segmentTypeByte;
                        segmentTypeByte = input.GetByte();
                        output.WriteByte(segmentTypeByte);
                    }
                    var segmentType = (JpegSegmentType)segmentTypeByte;


                    // the algorithm above stopped at a segment marker. This may be an opportunity
                    // to update or insert an App1-Xmp segment
                    if (!wroteXmp && segmentType == JpegSegmentType.App1)
                    {
                        // An App1 segment was encountered and might be a candidate for updating the Xmp
                        // Copy the segment (if it already contains Xmp, it will be updated)
                        wroteXmp = CopyOrUpdateApp1Segment(xmp, input, output, wroteXmp);
                    }
                    else if (!wroteXmp && segmentType != JpegSegmentType.Soi && segmentType != JpegSegmentType.App0)
                    {
                        // file begins with Soi (App0) (App1) ...
                        // At this point we have encountered a segment that should not be earlier than an App1.
                        // Also, the files does not contain an App1-Xmp segment yet.
                        // Therefore we must insert a new App1-Xmp segment now.
                        wroteXmp = InsertApp1XmpSegment(xmp, input, output, segmentIdentifier, segmentTypeByte);
                    }

                    // then continues with copying until the next segment or the end of the input
                }
            }
            catch(IOException ex)
            {
                // we expect a IOException when the sequential reader reaches the end of the original data
                if (ex.Message != "End of data reached.")
                    throw new JpegProcessingException("An error occured while trying to write Xml to the buffer.", ex);
            }
            catch (Exception ex)
            {
                throw new JpegProcessingException("An error occured while trying to write Xml to the buffer.", ex);
            }

            return output;
        }

        /// <summary>
        /// Copies an App1 segment and may replace it with Xmp.
        /// </summary>
        /// <param name="xmp">Xmp content that shall be used</param>
        /// <param name="input">Stream of bytes in the original file</param>
        /// <param name="output">Stream of bytes in the newly composed file</param>
        /// <param name="wroteXmp"></param>
        /// <returns>indicates if the Xmp content was written to this segment</returns>
        private static bool CopyOrUpdateApp1Segment(XDocument xmp, SequentialByteArrayReader input, MemoryStream output, bool wroteXmp)
        {
            // 1. read the segment index that encodes the segment length
            byte highByte = input.GetByte();
            byte lowByte = input.GetByte();
            int segmentLength = SegmentLengthFromBytes(highByte, lowByte, input.IsMotorolaByteOrder);

            // 2. read the rest of the segment
            byte[] segmentBytes = input.GetBytes(segmentLength);

            // 3. if this is the XMP segment, overwrite it with the new segment
            int preambleLength = XmpReader.JpegSegmentPreamble.Length;
            if (segmentBytes.Length >= preambleLength && XmpReader.JpegSegmentPreamble.Equals(Encoding.UTF8.GetString(segmentBytes, 0, preambleLength), StringComparison.OrdinalIgnoreCase))
            {
                // overwrite the payload
                segmentBytes = ByteArrayFromXmpXDocument(xmp);
                // and update the segment length!!
                byte[] lengthMark = BytesFromSegmentLength(segmentBytes.Length, input.IsMotorolaByteOrder);
                highByte = lengthMark[0];
                lowByte = lengthMark[1];
                // indicate that the Xmp was written to this segment
                wroteXmp = true;
            }

            // 4. only now write the entire segment (index + payload)
            output.WriteByte(highByte);
            output.WriteByte(lowByte);
            output.Write(segmentBytes, 0, segmentBytes.Length);

            return wroteXmp;
        }

        /// <summary>
        /// Inserts a new Xmp App1 segment to the output memory stream.
        /// The segment will be inserted -2 bytes from the current position.
        /// </summary>
        /// <param name="xmp">Xmp document to be inserted</param>
        /// <param name="input">Stream of bytes in the original file</param>
        /// <param name="output">Stream of bytes in the newly composed file</param>
        /// <param name="nextSegmentIdentifier">The identifier of the segment that began at position -2</param>
        /// <param name="nextSegmentTypeByte">The identifier of the segment that began at position -2</param>
        /// <returns>True, indicating that the Xmp content was inserted</returns>
        private static bool InsertApp1XmpSegment(XDocument xmp, SequentialByteArrayReader input, MemoryStream output, byte nextSegmentIdentifier, byte nextSegmentTypeByte)
        {
            // 1. remember the segment that comes after
            byte[] nextSegmentMarker = new byte[] { nextSegmentIdentifier, nextSegmentTypeByte };

            // 2. open a new App1 segment in place of the recently encountered segment
            output.Seek(-2, SeekOrigin.Current);
            // reference: http://dev.exiv2.org/projects/exiv2/wiki/The_Metadata_in_JPEG_files
            byte[] app1marker = new byte[] { 0xFF, 0xE1 };
            output.Write(app1marker, 0, app1marker.Length);

            // 3. prepare the new Xmp segment
            byte[] segmentBytes = ByteArrayFromXmpXDocument(xmp);
            byte[] lengthMark = BytesFromSegmentLength(segmentBytes.Length, input.IsMotorolaByteOrder);
            byte highByte = lengthMark[0];
            byte lowByte = lengthMark[1];
            // 4. write the new segment to the output
            output.WriteByte(highByte);
            output.WriteByte(lowByte);
            output.Write(segmentBytes, 0, segmentBytes.Length);

            // 5. now write the segment marker that was replaced
            // (in the next iteration it will just proceed with that segment)
            output.Write(nextSegmentMarker, 0, nextSegmentMarker.Length);
            return true;
        }

        /// <summary>
        /// Encodes an XDocument to bytes to be used as the payload of an App1 segment.
        /// </summary>
        /// <param name="xmp">Xmp document to be encoded</param>
        /// <returns>App1 segment payload</returns>
        private static byte[] ByteArrayFromXmpXDocument(XDocument xmp)
        {
            // we found the XMP segment. Now write the new XMP to the output
            MemoryStream xmpMS = new MemoryStream();
            // first the preamble "http://ns.adobe.com/xap/1.0/\0"
            byte[] preamble = UTF8Encoding.UTF8.GetBytes(XmpReader.JpegSegmentPreamble);
            xmpMS.Write(preamble, 0, preamble.Length);
            // now the XDocument WITHOUT Xml Declaration
            XmlWriterSettings settings = new XmlWriterSettings() { OmitXmlDeclaration = true };
            using (XmlWriter xw = XmlWriter.Create(xmpMS, settings))
            {
                xmp.WriteTo(xw);
            }
            // make it into a byte array
            return xmpMS.ToArray();
        }

        /// <summary>
        /// Computes the length of a segment payload from the high/low bytes of the index.
        /// (Segment length excludes the index bytes.)
        /// </summary>
        /// <param name="highByte">first byte of the index</param>
        /// <param name="lowByte">second byte of the index</param>
        /// <param name="motorolaBigEndian">byte order of the index</param>
        /// <returns></returns>
        private static int SegmentLengthFromBytes(byte highByte, byte lowByte, bool motorolaBigEndian)
        {
            // the segment length includes size bytes, so subtract two
            return -2 + ((motorolaBigEndian) ? (highByte << 8 | lowByte) : (highByte | lowByte << 8));
        }

        /// <summary>
        /// Encodes the length of a segment into the index bytes of the segment.
        /// </summary>
        /// <param name="length">Length of the payload (excludes the index)</param>
        /// <param name="motorolaBigEndian">byte order of the index</param>
        /// <returns>segment-index bytes (length 2)</returns>
        private static byte[] BytesFromSegmentLength(int length, bool motorolaBigEndian)
        {
            // the segment length includes the high & low bytes, so add 2
            byte[] bytes = BitConverter.GetBytes(length + 2);
            if (motorolaBigEndian)
                return new byte[] { bytes[1], bytes[0] };
            else
                return new byte[] { bytes[0], bytes[1] };
        }
    }
}

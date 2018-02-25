#region License
//
// Copyright 2002-2016 Drew Noakes
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
            SequentialByteArrayReader reader = new SequentialByteArrayReader(original);
            MemoryStream ms = new MemoryStream();

            try
            {
                bool wroteXmp = false;
                while (true)
                {
                    // Find the segment marker. Markers are zero or more 0xFF bytes, followed
                    // by a 0xFF and then a byte not equal to 0x00 or 0xFF.
                    var segmentIdentifier = reader.GetByte();
                    ms.WriteByte(segmentIdentifier);
                    var segmentTypeByte = reader.GetByte();
                    ms.WriteByte(segmentTypeByte);

                    // Read until we have a 0xFF byte followed by a byte that is not 0xFF or 0x00
                    while (segmentIdentifier != 0xFF || segmentTypeByte == 0xFF || segmentTypeByte == 0)
                    {
                        segmentIdentifier = segmentTypeByte;
                        segmentTypeByte = reader.GetByte();
                        ms.WriteByte(segmentTypeByte);
                    }

                    var segmentType = (JpegSegmentType)segmentTypeByte;

                    // is this App1?
                    // save the index of the first byte of this segment
                    // read the whole segment into a new byte[]
                    // evaluate if the segment contains the XMP metadata
                    // yes: write new xmp metadata to the output (discard the segment that was just read)
                    // no : write the whole segment to the output
                    // continue looping

                    if (!wroteXmp && segmentType == JpegSegmentType.App1)
                    {
                        // next 2-bytes are <segment-size>: [high-byte] [low-byte]
                        byte highByte = reader.GetByte();
                        byte lowByte = reader.GetByte();

                        //int segmentLength = -2 + ((reader.IsMotorolaByteOrder) ? (highByte << 8 | lowByte) : (highByte | lowByte << 8));// (includes size bytes, so subtract two)
                        int segmentLength = SegmentLengthFromBytes(highByte, lowByte, reader.IsMotorolaByteOrder) - 2;

                        // the rest of the segment is only read - at first
                        byte[] segmentBytes = reader.GetBytes(segmentLength);

                        // if this is the XMP segment, overwrite the byte[] with the new segment
                        int preambleLength = XmpReader.JpegSegmentPreamble.Length;
                        if (segmentBytes.Length >= preambleLength && XmpReader.JpegSegmentPreamble.Equals(Encoding.UTF8.GetString(segmentBytes, 0, preambleLength), StringComparison.OrdinalIgnoreCase))
                        {
                            segmentBytes = ByteArrayFromXmpXDocument(xmp);
                            // also update the segment length!!
                            byte[] lengthMark = BytesFromSegmentLength(segmentBytes.Length + 2, reader.IsMotorolaByteOrder);
                            highByte = lengthMark[0];
                            lowByte = lengthMark[1];
                            // no one will ever know this was not the original segment
                            wroteXmp = true;
                        }
                        // write the segment
                        ms.WriteByte(highByte);
                        ms.WriteByte(lowByte);
                        ms.Write(segmentBytes, 0, segmentBytes.Length);
                    }
                    else if (!wroteXmp && segmentType != JpegSegmentType.Soi && segmentType != JpegSegmentType.App0) // file begins with Soi (App0) App1 ...
                    {
                        // we have encountered a segment that should not be earlier than App1. Therfore we must insert a new App1 with the Xmp right now. (The file does not contain an App1 segement yet.)
                        // go back and overwrite the recently encountered marker (cache what it was!)
                        // create a new marker App1 http://dev.exiv2.org/projects/exiv2/wiki/The_Metadata_in_JPEG_files
                        // build Xmp byte[]
                        // ...
                        // end by writing the marker that was substituted before

                        // remember the segment that comes after
                        byte[] nextSegmentMarker = new byte[] { segmentIdentifier, segmentTypeByte };
                        ms.Seek(-2, SeekOrigin.Current);
                        // open a new App1 segment
                        byte[] app1marker = new byte[] { 0xFF, 0xE1 };
                        ms.Write(app1marker, 0, app1marker.Length);
                        // build Xmp segment
                        byte[] segmentBytes = ByteArrayFromXmpXDocument(xmp);
                        // calculate a length marker
                        byte[] lengthMark = BytesFromSegmentLength(segmentBytes.Length + 2, reader.IsMotorolaByteOrder);
                        byte highByte = lengthMark[0];
                        byte lowByte = lengthMark[1];
                        // write the segment to the output
                        ms.WriteByte(highByte);
                        ms.WriteByte(lowByte);
                        ms.Write(segmentBytes, 0, segmentBytes.Length);
                        // remember that we're done
                        wroteXmp = true;
                        // write the segment marker that we replaced
                        ms.Write(nextSegmentMarker, 0, nextSegmentMarker.Length);
                        // in the next loop it will just proceed
                    }
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

            return ms;
        }
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
        private static int SegmentLengthFromBytes(byte highByte, byte lowByte, bool motorolaBigEndian)
        {
            return ((motorolaBigEndian) ? (highByte << 8 | lowByte) : (highByte | lowByte << 8));
        }
        private static byte[] BytesFromSegmentLength(int length, bool motorolaBigEndian)
        {
            byte[] bytes = BitConverter.GetBytes(length);
            if (motorolaBigEndian)
                return new byte[] { bytes[1], bytes[0] };
            else
                return new byte[] { bytes[0], bytes[1] };
        }

        ///// <exception cref="JpegProcessingException"/>
        ///// <exception cref="System.IO.IOException"/>
        //public static void Process([NotNull] Stream stream, [CanBeNull] IEnumerable<IJpegSegmentMetadataReader> readers = null)
        //{
        //    if (readers == null)
        //        readers = _allReaders;

        //    var segmentTypes = new HashSet<JpegSegmentType>(readers.SelectMany(reader => reader.GetSegmentTypes()));
        //    var segmentData = JpegSegmentReader.ReadSegments(new SequentialStreamReader(stream), segmentTypes);
        //    return ProcessJpegSegmentData(readers, segmentData);
        //}

        //public static void ProcessJpegSegmentData(IEnumerable<IJpegSegmentMetadataReader> readers, JpegSegmentData segmentData)
        //{
        //    // Pass the appropriate byte arrays to each reader.
        //    return (from reader in readers
        //            from segmentType in reader.GetSegmentTypes()
        //            from directory in reader.ReadJpegSegments(segmentData.GetSegments(segmentType), segmentType)
        //            select directory)
        //            .ToList();
        //}
    }
}

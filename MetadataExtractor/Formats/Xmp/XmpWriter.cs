using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#if NET35
using FragmentList = System.Collections.Generic.IList<MetadataExtractor.Formats.Jpeg.JpegFragment>;
#else
using FragmentList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Formats.Jpeg.JpegFragment>;
#endif

namespace MetadataExtractor.Formats.Xmp
{
    public sealed class XmpWriter : IJpegFragmentMetadataWriter
    {
        /// <summary>
        /// Specifies the type of metadata that this MetadataWriter can handle.
        /// </summary>
        Type IJpegFragmentMetadataWriter.MetadataType => typeof(XmpDirectory);

        /// <summary>
        /// Updates a list of JpegFragments with new metadata.
        /// <para>
        /// An existing App1 Xmp fragment will be updated. If none is found, a new segment will be
        /// inserted before the first fragment that is not one of {Soi, App0, App1}
        /// </para>
        /// </summary>
        /// <param name="fragments">Original file fragmets</param>
        /// <param name="metadata">The Xmp metadata that shall be written</param>
        /// <param name="isMotorolaByteOrder">
        /// Indicates if the collection of fragments is encoded using MotorolaByteOrder
        /// </param>
        /// <returns>A new list of JpegFragments</returns>
        public List<JpegFragment> UpdateFragments([NotNull] FragmentList fragments, [NotNull] object metadata, bool isMotorolaByteOrder)
        {
            JpegFragment metadataFragment;
            List<JpegFragment> output = new List<JpegFragment>();
            bool wroteData = false;

            if (metadata is XDocument)
            {
                byte[] payloadBytes = PayloadBytesFromXmpXDocument((XDocument)metadata);
                byte[] segmentBytes = SegmentBytesFromPayload(JpegSegmentType.App1, payloadBytes, isMotorolaByteOrder);
                metadataFragment = new JpegFragment(segmentBytes);
            }
            else
            {
                throw new ArgumentException($"XmpWriter expects metadata to be of type XmpDirectory, but was given {metadata.GetType()}.");
            }

            // Walk over existing fragment and replace or insert the new metadata fragment
            for (int i = 0; i < fragments.Count; i++)
            {
                JpegFragment currentFragment = fragments[i];

                if (!wroteData && currentFragment.IsSegment)
                {
                    JpegSegmentType currentType = currentFragment.Segment.Type;

                    // if this is an existing App1 XMP fragment, overwrite it with the new fragment
                    if (currentType == JpegSegmentType.App1 && currentFragment.Segment.Bytes.Length > XmpReader.JpegSegmentPreamble.Length)
                    {
                        // This App1 segment could be a candidate for overwriting.
                        // Read the encountered segment payload to check if it contains the Xmp preamble
                        string potentialPreamble = Encoding.UTF8.GetString(currentFragment.Segment.Bytes, 0, XmpReader.JpegSegmentPreamble.Length);
                        if (potentialPreamble.Equals(XmpReader.JpegSegmentPreamble, StringComparison.OrdinalIgnoreCase))
                        {
                            // The existing Xmp App1 fragment will be replaced with the new fragment
                            currentFragment = metadataFragment;
                            wroteData = true;
                        }
                    }
                    else if (currentType != JpegSegmentType.Soi && currentType != JpegSegmentType.App0)
                    {
                        // file begins with Soi (App0) (App1) ...
                        // At this point we have encountered a segment that should not be earlier than an App1.
                        // Also, the files does not contain an App1-Xmp segment yet.
                        // Therefore we must insert a new App1-Xmp segment now.
                        output.Add(metadataFragment);
                        wroteData = true;
                    }
                }
                output.Add(currentFragment);
            }

            return output;
        }

        /// <summary>
        /// Concatenates the JpegSegment mark, segment length and payload bytes.
        /// </summary>
        /// <param name="type">The type of JpegSegment to encode</param>
        /// <param name="payloadBytes">Payload of the JpegSegment</param>
        /// <param name="isMotorolaByteOrder">Indicates the byte order for the segment length encoding</param>
        /// <returns>Byte array of the entire segment</returns>
        private static byte[] SegmentBytesFromPayload(JpegSegmentType type, byte[] payloadBytes, bool isMotorolaByteOrder)
        {
            byte[] segmentBytes = new byte[4 + payloadBytes.Length];
            byte[] lengthMark = EncodeSegmentLength(payloadBytes.Length, isMotorolaByteOrder);

            segmentBytes[0] = 0xFF;
            segmentBytes[1] = (byte)type;
            segmentBytes[2] = lengthMark[0];
            segmentBytes[3] = lengthMark[1];
            payloadBytes.CopyTo(segmentBytes, 4);

            return segmentBytes;
        }

        /// <summary>
        /// Encodes an XDocument to bytes to be used as the payload of an App1 segment.
        /// </summary>
        /// <param name="xmp">Xmp document to be encoded</param>
        /// <returns>App1 segment payload</returns>
        private static byte[] PayloadBytesFromXmpXDocument([NotNull] XDocument xmp)
        {
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
        private static int DecodeSegmentLength(byte highByte, byte lowByte, bool motorolaBigEndian)
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
        private static byte[] EncodeSegmentLength(int length, bool motorolaBigEndian)
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

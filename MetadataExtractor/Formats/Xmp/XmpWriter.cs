using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// <returns>A new list of JpegFragments</returns>
        public List<JpegFragment> UpdateFragments([NotNull] FragmentList fragments, [NotNull] object metadata)
        {
            JpegFragment metadataFragment;
            List<JpegFragment> output = new List<JpegFragment>();
            bool wroteData = false;
            int insertPosition = 0;

            if (metadata is XDocument)
            {
                byte[] payloadBytes = EncodeXmpToPayloadBytes((XDocument)metadata);
                JpegSegment metadataSegment = new JpegSegment(JpegSegmentType.App1, payloadBytes, offset: 0);
                metadataFragment = JpegFragment.FromJpegSegment(metadataSegment);
            }
            else
            {
                throw new ArgumentException($"XmpWriter expects metadata to be of type XmpDirectory, but was given {metadata.GetType()}.");
            }

            // First look for any potential Xmp fragment, insert only if none is found

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
                    else if (insertPosition == 0 && currentType != JpegSegmentType.Soi && currentType != JpegSegmentType.App0)
                    {
                        // file begins with Soi (App0) (App1) ...
                        // At this point we have encountered a segment that should not be earlier than an App1.
                        // But there could be another Xmp segment, so we just make a note of this position
                        insertPosition = i;
                    }
                }
                output.Add(currentFragment);
            }

            if (!wroteData)
            {
                // The files does not contain an App1-Xmp segment yet.
                // Therefore we must insert a new App1-Xmp segment at the previously determined position.
                output.Insert(insertPosition, metadataFragment);
                wroteData = true;
            }

            return output;
        }

        /// <summary>
        /// Encodes an XDocument to bytes to be used as the payload of an App1 segment.
        /// </summary>
        /// <param name="xmpDoc">Xmp document to be encoded</param>
        /// <param name="writeable">Indicates if the Xmp packet shall be marked as writable.</param>
        /// <returns>App1 segment payload</returns>
        public static byte[] EncodeXmpToPayloadBytes([NotNull] XDocument xmpDoc, bool writeable=true)
        {
            // constant parts
            byte[] preamble = Encoding.UTF8.GetBytes(XmpReader.JpegSegmentPreamble);
            byte[] xpacketHeader = Encoding.UTF8.GetBytes("<?xpacket begin=\"\uFEFF\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>");
            byte[] xpacketTrailer = Encoding.UTF8.GetBytes($"<?xpacket end='{ (writeable ? 'w' : 'r') }'?>");

            MemoryStream xmpMS = new MemoryStream();
            // 1. preamble "http://ns.adobe.com/xap/1.0/\0"
            xmpMS.Write(preamble, 0, preamble.Length);

            // 2. xpacket header
            xmpMS.Write(xpacketHeader, 0, xpacketHeader.Length);

            // 3. serialized Xmp xml
            XmlWriterSettings settings = new XmlWriterSettings() { OmitXmlDeclaration = true };
            using (XmlWriter xmlWriter = XmlWriter.Create(xmpMS, settings))
            {
                xmpDoc.WriteTo(xmlWriter);
            }

            // 4. whitespace padding
            byte[] whitespace = Encoding.UTF8.GetBytes(CreateWhitespace());
            xmpMS.Write(whitespace, 0, whitespace.Length);

            // 5. xpacket trailer
            xmpMS.Write(xpacketTrailer, 0, xpacketTrailer.Length);

            return xmpMS.ToArray();
        }

        /// <summary>
        /// Creates a string of whitespace with linebreaks for padding within xpacket.
        /// </summary>
        /// <param name="size">Desired total size of whitespace</param>
        /// <returns>String of whitespace with newline character in each line of 100 chars</returns>
        public static string CreateWhitespace(int size=4096)
        {
            var line = '\u000A' + new String('\u0020', 99);
            return string.Concat(Enumerable.Repeat(line, (int)Math.Ceiling(size / 100.0))).Substring(0, size);
        }
    }
}

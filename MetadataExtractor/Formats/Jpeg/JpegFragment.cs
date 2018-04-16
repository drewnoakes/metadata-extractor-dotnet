using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>
    /// Contains a fragment of JPEG data which may or may not be a JpegSegment.
    /// </summary>
    /// <seealso cref="JpegSegmentReader"/>
    /// <author>Michael Osthege</author>
    public sealed class JpegFragment
    {
        /// <summary>
        /// Indicates if this fragment is a JpegSegment.
        /// </summary>
        public bool IsSegment { get { return Segment != null; } }

        /// <summary>
        /// JpegSegment interpretation of this fragment.
        /// </summary>
        [CanBeNull]
        public JpegSegment Segment { get; }

        /// <summary>
        /// All bytes that make up the JpegFragment. (Includes potential JpegSegment marker + size)
        /// </summary>
        [NotNull]
        public byte[] Bytes { get; }

        /// <summary>
        /// Create a JpegFragment from the fragment bytes.
        /// </summary>
        /// <param name="bytes">All bytes that make up the fragment.</param>
        /// <param name="segment">Optional JpegSegment interpretation of this fragment.</param>
        public JpegFragment([NotNull] byte[] bytes, [CanBeNull] JpegSegment segment = null)
        {
            Bytes = bytes;
            Segment = segment;
        }

        /// <summary>
        /// Infers the marker and segment size bytes that correspond to the JpegSegment.
        /// </summary>
        /// <param name="segment">A JpegSegment</param>
        /// <param name="isMotorolaByteOrder">Indicates if the collection of fragments is encoded using MotorolaByteOrder</param>
        /// <returns>A JpegFragment that is the concatenation of JpegSegment marker, size bytes and payload.</returns>
        public static JpegFragment FromJpegSegment(JpegSegment segment, bool isMotorolaByteOrder)
        {
            byte[] fragmentBytes = new byte[2 + 2 + segment.Bytes.Length];

            // Segment marker
            fragmentBytes[0] = 0xFF;
            fragmentBytes[1] = (byte)segment.Type;

            // Segment size
            byte[] sizeBytes = BitConverter.GetBytes(segment.Bytes.Length + 2);
            if (isMotorolaByteOrder)
                new byte[] { sizeBytes[1], sizeBytes[0] }.CopyTo(fragmentBytes, 0);
            else
                new byte[] { sizeBytes[0], sizeBytes[1] }.CopyTo(fragmentBytes, 0);

            // Segment payload
            segment.Bytes.CopyTo(fragmentBytes, 4);

            return new JpegFragment(fragmentBytes, segment);
        }
    }
}

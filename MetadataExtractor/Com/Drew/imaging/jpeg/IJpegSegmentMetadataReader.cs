using System.Collections.Generic;
using JetBrains.Annotations;

namespace Com.Drew.Imaging.Jpeg
{
    /// <summary>Defines an object that extracts metadata from in JPEG segments.</summary>
    public interface IJpegSegmentMetadataReader
    {
        /// <summary>Gets the set of JPEG segment types that this reader is interested in.</summary>
        [NotNull]
        IEnumerable<JpegSegmentType> GetSegmentTypes();

        /// <summary>Extracts metadata from all instances of a particular JPEG segment type.</summary>
        /// <param name="segments">
        /// A sequence of byte arrays from which the metadata should be extracted. These are in the order
        /// encountered in the original file.
        /// </param>
        /// <param name="metadata">The <see cref="Com.Drew.Metadata.Metadata"/> object into which extracted values should be merged.</param>
        /// <param name="segmentType">The <see cref="JpegSegmentType"/> being read.
        /// </param>
        void ReadJpegSegments([NotNull] IEnumerable<byte[]> segments, [NotNull] Metadata.Metadata metadata, [NotNull] JpegSegmentType segmentType);
    }
}

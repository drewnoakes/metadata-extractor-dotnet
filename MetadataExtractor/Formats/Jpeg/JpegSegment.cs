using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Jpeg
{
    public sealed class JpegSegment
    {
        public JpegSegment(JpegSegmentType segmentType, [NotNull] byte[] segmentBytes, long segmentPosition)
        {
            Type = segmentType;
            Bytes = segmentBytes;
            StartPosition = segmentPosition;
        }

        public JpegSegment(JpegSegmentType segmentType, byte[] bytes)
        {
            Type = segmentType;
            Bytes = bytes;
            StartPosition = 0;
        }

        public JpegSegmentType Type { get; }

        public byte[] Bytes { get; }

        public long StartPosition { get; }
    }
}

using JetBrains.Annotations;

namespace MetadataExtractor.Util
{
    internal static class ByteConvert
    {
        [Pure]
        public static int ToInt32BigEndian([NotNull] byte[] bytes)
        {
            return
                bytes[0] << 24 |
                bytes[1] << 16 |
                bytes[2] << 8 |
                bytes[3];
        }

        [Pure]
        public static int ToInt32LittleEndian([NotNull] byte[] bytes)
        {
            return
                bytes[0] |
                bytes[1] << 8 |
                bytes[2] << 16 |
                bytes[3] << 24;
        }
    }
}
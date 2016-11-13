using JetBrains.Annotations;

namespace MetadataExtractor.Util
{
    public static class ByteArrayUtil
    {
        [Pure]
        public static bool StartsWith([NotNull] this byte[] source, [NotNull] byte[] pattern)
        {
            if (source.Length < pattern.Length)
                return false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < pattern.Length; i++)
                if (source[i] != pattern[i])
                    return false;

            return true;
        }
    }
}
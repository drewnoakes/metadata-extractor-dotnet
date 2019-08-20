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

        public static bool EqualTo([NotNull] this byte[] source, [NotNull] byte[] compare)
        {
            // If not the same length, bail out
            if (source.Length != compare.Length)
                return false;

            // If they are the same object, bail out
            if (ReferenceEquals(source, compare))
                return true;

            // Loop all values and compare
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] != compare[i])
                    return false;
            }

            // If we got here, equal
            return true;
        }
    }
}
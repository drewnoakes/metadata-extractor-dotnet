using System;
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

            return source.AsSpan().StartsWith(pattern);
        }
    }
}
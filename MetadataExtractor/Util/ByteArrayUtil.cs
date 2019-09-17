// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using JetBrains.Annotations;

namespace MetadataExtractor.Util
{
    public static class ByteArrayUtil
    {
        [Pure]
        public static bool StartsWith(this byte[] source, byte[] pattern)
        {
            if (source.Length < pattern.Length)
                return false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < pattern.Length; i++)
                if (source[i] != pattern[i])
                    return false;

            return true;
        }

        [Pure]
        public static bool EqualTo(this byte[] source, byte[] compare)
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

// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Util;

internal static class ByteArrayExtensions
{
    public static bool RegionEquals(this byte[] bytes, int offset, int count, byte[] comparand)
    {
        if (offset < 0 ||                   // invalid arg
            count < 0 ||                    // invalid arg
            bytes.Length < offset + count)  // extends beyond end
            return false;

        for (int i = 0, j = offset; i < count; i++, j++)
        {
            if (bytes[j] != comparand[i])
                return false;
        }

        return true;
    }
}

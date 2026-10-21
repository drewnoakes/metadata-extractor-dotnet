// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Mkv;

internal static class DataParser
{
    public static int GetElementId(SequentialReader reader)
    {
        int value = reader.GetByte();
        int length = GetVintLength(value, 4);
        for (var i = 1; i < length; i++)
            value = value << 8 | reader.GetByte();
        if (value == (1 << (7 * length + 1)) - 1)
            throw new IOException("Reserved EBML element ID.");
        return value;
    }

    public static long GetElementSize(SequentialReader reader)
    {
        int first = reader.GetByte();
        int length = GetVintLength(first, 8);
        long value = first & ((1 << (8 - length)) - 1);
        for (var i = 1; i < length; i++)
            value = value << 8 | reader.GetByte();
        return value == (1L << (7 * length)) - 1 ? -1 : value;
    }

    private static int GetVintLength(int first, int maximum)
    {
        var length = 1;
        var marker = 0x80;
        while ((first & marker) == 0)
        {
            if (++length > maximum)
                throw new IOException("Invalid EBML variable-length integer width.");
            marker >>= 1;
        }
        return length;
    }

    public static ulong GetUnsignedInteger(SequentialReader reader, long size)
    {
        if (size is < 0 or > 8)
            throw new IOException("EBML integers must contain zero to eight bytes.");
        ulong value = 0;
        for (long i = 0; i < size; i++)
            value = value << 8 | reader.GetByte();
        return value;
    }

    public static double GetDouble(SequentialReader reader, long size) => size switch
    {
        0 => 0,
        4 => reader.GetFloat32(),
        8 => reader.GetDouble64(),
        _ => throw new IOException("EBML floats must contain zero, four, or eight bytes.")
    };

    public static int GetByteCount(long size)
    {
        if (size is < 0 or > int.MaxValue)
            throw new IOException("EBML value is too large to store in memory.");
        return (int)size;
    }
}

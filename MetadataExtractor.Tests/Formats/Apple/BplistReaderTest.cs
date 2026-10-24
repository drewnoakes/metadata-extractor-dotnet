// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers.Binary;
using MetadataExtractor.Formats.Exif.Makernotes;

namespace MetadataExtractor.Formats.Apple;

public sealed class BplistReaderTest
{
    [Theory]
    [InlineData(1, false, 8)]
    [InlineData(1, true, 8)]
    [InlineData(2, false, 8)]
    [InlineData(2, true, 8)]
    [InlineData(2, true, 260)]
    public void Parse_ReadsOffsetTableFromItsStartAndPreservesTopObject(int offsetWidth, bool nonzeroTopObject, int objectStart)
    {
        var bytes = CreateBplist(offsetWidth, nonzeroTopObject, objectStart);

        var result = BplistReader.Parse(bytes);

        var root = Assert.IsType<Dictionary<byte, byte>>(result.GetTopObject());
        var pair = Assert.Single(root);
        Assert.Equal(nonzeroTopObject ? (byte)0 : (byte)1, pair.Key);
        Assert.Equal(nonzeroTopObject ? (byte)1 : (byte)2, pair.Value);
        Assert.Equal("k", Assert.IsType<string>(result.Get(pair.Key)));
        Assert.Equal((byte)42, Assert.IsType<byte>(result.Get(pair.Value)));
    }

    [Theory]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(31)]
    [InlineData(32)]
    [InlineData(39)]
    public void Parse_TruncatedTrailerThrowsIOException(int length)
    {
        var bytes = new byte[length];
        "bplist00"u8.CopyTo(bytes);

        Assert.Throws<IOException>(() => BplistReader.Parse(bytes));
    }

    // These intentionally exercise counts which must be rejected BEFORE allocating an
    // offsets array. Do not run against the unhardened parser (which allocates by count).
    [Theory]
    [InlineData(-1L)]
    [InlineData(0L)]
    [InlineData(64L)]
    [InlineData(2147483647L)]
    [InlineData(2147483648L)]
    [InlineData(4294967297L)]
    [InlineData(long.MaxValue)]
    public void Parse_InvalidObjectCountThrowsIOException(long count)
    {
        var bytes = CreateBplist();
        SetTrailerInt64(bytes, 8, count);

        Assert.Throws<IOException>(() => BplistReader.Parse(bytes));
    }

    [Theory]
    [InlineData(-1L)]
    [InlineData(3L)]
    [InlineData(2147483648L)]
    [InlineData(long.MaxValue)]
    public void Parse_InvalidTopObjectThrowsIOException(long topObject)
    {
        var bytes = CreateBplist();
        SetTrailerInt64(bytes, 16, topObject);

        Assert.Throws<IOException>(() => BplistReader.Parse(bytes));
    }

    [Theory]
    [InlineData(-1L)]
    [InlineData(0L)]
    [InlineData(7L)]
    [InlineData(16L)] // Only two table bytes remain before the trailer; three are required.
    [InlineData(18L)] // Trailer starts here.
    [InlineData(49L)] // Last byte of the complete input.
    [InlineData(50L)]
    [InlineData(long.MaxValue)]
    public void Parse_InvalidOffsetTableOffsetThrowsIOException(long offset)
    {
        var bytes = CreateBplist();
        SetTrailerInt64(bytes, 24, offset);

        Assert.Throws<IOException>(() => BplistReader.Parse(bytes));
    }

    [Theory]
    [InlineData(6, 0)]
    [InlineData(6, 3)]
    [InlineData(6, 4)]
    [InlineData(6, 8)]
    [InlineData(6, 255)]
    [InlineData(7, 0)]
    [InlineData(7, 2)]
    [InlineData(7, 4)]
    [InlineData(7, 8)]
    [InlineData(7, 255)]
    public void Parse_UnsupportedOffsetOrReferenceWidthThrowsIOException(int trailerField, byte width)
    {
        var bytes = CreateBplist();
        bytes[bytes.Length - 32 + trailerField] = width;

        Assert.Throws<IOException>(() => BplistReader.Parse(bytes));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)] // Inside the header.
    [InlineData(15)] // First offset-table byte, not an object.
    [InlineData(16)]
    [InlineData(18)] // Inside the trailer.
    [InlineData(49)]
    [InlineData(255)]
    public void Parse_ObjectOffsetOutsideObjectAreaThrowsIOException(byte offset)
    {
        var bytes = CreateBplist();
        bytes[15] = offset;

        Assert.Throws<IOException>(() => BplistReader.Parse(bytes));
    }

    [Theory]
    [InlineData(8)]
    [InlineData(39)]
    [InlineData(50)]
    public void AppleRunTime_MalformedTrailerReportsDirectoryError(int length)
    {
        var bytes = length == 50 ? CreateBplist() : new byte[length];
        "bplist00"u8.CopyTo(bytes);
        if (length == 50)
            SetTrailerInt64(bytes, 24, -1);

        var directory = AppleRunTimeMakernoteDirectory.Parse(bytes);

        Assert.True(directory.HasError);
        Assert.NotEmpty(directory.Errors);
        Assert.False(directory.ContainsTag(AppleRunTimeMakernoteDirectory.TagFlags));
        Assert.False(directory.ContainsTag(AppleRunTimeMakernoteDirectory.TagEpoch));
        Assert.False(directory.ContainsTag(AppleRunTimeMakernoteDirectory.TagScale));
        Assert.False(directory.ContainsTag(AppleRunTimeMakernoteDirectory.TagValue));
    }

    private static byte[] CreateBplist(int offsetWidth = 1, bool nonzeroTopObject = false, int objectStart = 8)
    {
        byte[] objects = nonzeroTopObject
            ? [0x51, (byte)'k', 0x10, 42, 0xD1, 0, 1]
            : [0xD1, 1, 2, 0x51, (byte)'k', 0x10, 42];
        int[] offsets = nonzeroTopObject
            ? [objectStart, objectStart + 2, objectStart + 4]
            : [objectStart, objectStart + 3, objectStart + 5];
        int tableStart = objectStart + objects.Length;
        var bytes = new byte[tableStart + offsets.Length * offsetWidth + 32];
        "bplist00"u8.CopyTo(bytes);
        objects.CopyTo(bytes, objectStart);
        for (int i = 0; i < offsets.Length; i++)
        {
            if (offsetWidth == 1)
                bytes[tableStart + i] = checked((byte)offsets[i]);
            else
                BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(tableStart + i * 2), checked((ushort)offsets[i]));
        }

        bytes[bytes.Length - 32 + 6] = checked((byte)offsetWidth);
        bytes[bytes.Length - 32 + 7] = 1;
        SetTrailerInt64(bytes, 8, offsets.Length);
        SetTrailerInt64(bytes, 16, nonzeroTopObject ? 2 : 0);
        SetTrailerInt64(bytes, 24, tableStart);
        return bytes;
    }

    private static void SetTrailerInt64(byte[] bytes, int fieldOffset, long value) =>
        BinaryPrimitives.WriteInt64BigEndian(bytes.AsSpan(bytes.Length - 32 + fieldOffset), value);
}

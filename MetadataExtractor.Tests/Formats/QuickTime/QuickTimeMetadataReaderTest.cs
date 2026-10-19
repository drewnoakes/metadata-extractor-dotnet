// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime;

public sealed class QuickTimeMetadataReaderTest
{
    [Fact]
    public void ReadsUserDataMetadataFromUdta()
    {
        // Build a minimal MP4 atom tree:
        //   moov
        //     udta
        //       meta (FullBox) > ilst > ©nam (Title), ©cmt (Comment)
        //       Xtra > WM/SubTitle, WM/SharedUserRating, WM/Provider (unrecognised), WM/Category, WM/Mood
        var ilst = Box("ilst", Concat(
            Box("\u00A9nam", DataAtom("My Title")),
            Box("\u00A9cmt", DataAtom("My Comment"))));

        // 'meta' is a FullBox, so its payload begins with a 4-byte version/flags field.
        var meta = Box("meta", Concat([0, 0, 0, 0], ilst));

        var xtra = Box("Xtra", Concat(
            XtraStringEntry("WM/SubTitle", "My Subtitle"),
            XtraRatingEntry("WM/SharedUserRating", 75),
            XtraStringEntry("WM/Provider", "ignored"), // unrecognised key, must not misalign parsing
            XtraStringEntry("WM/Category", "Rock", "Pop"),
            XtraStringEntry("WM/Mood", "Happy")));

        var moov = Box("moov", Box("udta", Concat(meta, xtra)));

        var directories = QuickTimeMetadataReader.ReadMetadata(new MemoryStream(moov));

        var directory = directories.OfType<QuickTimeMetadataHeaderDirectory>().Single();

        Assert.Equal("My Title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.Equal("My Comment", directory.GetString(QuickTimeMetadataHeaderDirectory.TagComment));
        Assert.Equal("My Subtitle", directory.GetString(QuickTimeMetadataHeaderDirectory.TagSubtitle));
        Assert.Equal(75L, directory.GetInt64(QuickTimeMetadataHeaderDirectory.TagRating));
        Assert.Equal("Rock | Pop", directory.GetString(QuickTimeMetadataHeaderDirectory.TagCategory));
        Assert.Equal("Happy", directory.GetString(QuickTimeMetadataHeaderDirectory.TagMood));
    }

    [Theory]
    [InlineData("2025-04-11T20:01:03+0800", 480, 0)]
    [InlineData("2025-04-11T20:01:03-05:30", -330, 0)]
    [InlineData("2025-04-11T20:01:03Z", 0, 0)]
    [InlineData("2025-04-11T20:01:03.1234567+08:00", 480, 1234567)]
    public void ReadsKeyedDatesWithOffsetsWithoutLosingWallTime(string text, int offsetMinutes, int fractionTicks)
    {
        using var stream = new MemoryStream(KeyedDates(text, text));
        var directory = QuickTimeMetadataReader.ReadMetadata(stream).OfType<QuickTimeMetadataHeaderDirectory>().Single();
        var wallTime = new DateTime(2025, 4, 11, 20, 1, 3).AddTicks(fractionTicks);
        var offset = TimeSpan.FromMinutes(offsetMinutes);

        foreach (var tag in new[] { QuickTimeMetadataHeaderDirectory.TagCreationDate, QuickTimeMetadataHeaderDirectory.TagLocationDate })
        {
            var stored = Assert.IsType<DateTimeOffset>(directory.GetObject(tag));
            Assert.Equal(wallTime, stored.DateTime);
            Assert.Equal(offset, stored.Offset);
            Assert.Equal(DateTime.SpecifyKind(wallTime - offset, DateTimeKind.Utc), stored.UtcDateTime);
            Assert.True(directory.TryGetDateTimeOffset(tag, out var actual));
            Assert.True(stored.EqualsExact(actual));
            Assert.True(stored.EqualsExact(directory.GetDateTimeOffset(tag)));
            Assert.Equal(stored.ToString(), directory.GetString(tag));
            Assert.Equal(stored.ToString(), directory.GetDescription(tag));
            Assert.Equal(DateTimeKind.Local, directory.GetDateTime(tag).Kind);
            Assert.Equal(stored.UtcDateTime, directory.GetDateTime(tag).ToUniversalTime());
        }
        Assert.False(directory.HasError);
    }

    [Theory]
    [InlineData("2025-04-11T20:01:03", 0)]
    [InlineData("2025-04-11T20:01:03.1234567", 1234567)]
    public void ReadsKeyedDatesWithoutOffsetsAsUnspecifiedDateTime(string text, int fractionTicks)
    {
        using var stream = new MemoryStream(KeyedDates(text, text));
        var directory = QuickTimeMetadataReader.ReadMetadata(stream).OfType<QuickTimeMetadataHeaderDirectory>().Single();
        var expected = new DateTime(2025, 4, 11, 20, 1, 3).AddTicks(fractionTicks);

        foreach (var tag in new[] { QuickTimeMetadataHeaderDirectory.TagCreationDate, QuickTimeMetadataHeaderDirectory.TagLocationDate })
        {
            var stored = Assert.IsType<DateTime>(directory.GetObject(tag));
            Assert.Equal(expected, stored);
            Assert.Equal(DateTimeKind.Unspecified, stored.Kind);
            Assert.Equal(expected, directory.GetDateTime(tag));
            Assert.Equal(DateTimeKind.Unspecified, directory.GetDateTime(tag).Kind);
            Assert.False(directory.TryGetDateTimeOffset(tag, out var actual));
            Assert.Equal(default(DateTimeOffset), actual);
            Assert.Throws<MetadataException>(() => directory.GetDateTimeOffset(tag));
        }
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsKeyedDatesWithIndependentOffsetPresence(bool creationHasOffset)
    {
        const string withOffset = "2025-04-11T20:01:03+0800";
        const string withoutOffset = "2025-04-12T06:07:08";
        using var stream = new MemoryStream(KeyedDates(
            creationHasOffset ? withOffset : withoutOffset,
            creationHasOffset ? withoutOffset : withOffset));
        var directory = QuickTimeMetadataReader.ReadMetadata(stream).OfType<QuickTimeMetadataHeaderDirectory>().Single();
        var offsetTag = creationHasOffset ? QuickTimeMetadataHeaderDirectory.TagCreationDate : QuickTimeMetadataHeaderDirectory.TagLocationDate;
        var unknownTag = creationHasOffset ? QuickTimeMetadataHeaderDirectory.TagLocationDate : QuickTimeMetadataHeaderDirectory.TagCreationDate;

        var known = Assert.IsType<DateTimeOffset>(directory.GetObject(offsetTag));
        Assert.Equal(new DateTime(2025, 4, 11, 20, 1, 3), known.DateTime);
        Assert.Equal(TimeSpan.FromHours(8), known.Offset);
        Assert.Equal(new DateTime(2025, 4, 11, 12, 1, 3, DateTimeKind.Utc), known.UtcDateTime);
        var unknown = Assert.IsType<DateTime>(directory.GetObject(unknownTag));
        Assert.Equal(new DateTime(2025, 4, 12, 6, 7, 8), unknown);
        Assert.Equal(DateTimeKind.Unspecified, unknown.Kind);
        Assert.False(directory.TryGetDateTimeOffset(unknownTag, out var actual));
        Assert.Equal(default(DateTimeOffset), actual);
        Assert.False(directory.HasError);
    }

    [Theory]
    [InlineData("2025-02-30T20:01:03+0800", "2025-04-11T20:01:03+0800")]
    [InlineData("2025-04-11T20:01:03+0800", "2025-02-30T20:01:03+0800")]
    [InlineData("not a date", "2025-04-11T20:01:03")]
    [InlineData("2025-04-11T20:01:03", "not a date")]
    public void ReadKeyedDates_InvalidDate_ThrowsFormatException(string creationDate, string locationDate)
    {
        using var stream = new MemoryStream(KeyedDates(creationDate, locationDate));

        Assert.Throws<FormatException>(() => QuickTimeMetadataReader.ReadMetadata(stream));
    }

    private static byte[] KeyedDates(string creationDate, string locationDate)
    {
        // moov/meta uses indexed keys, unlike the iTunes-style moov/udta/meta fixture.
        var keyHeader = new byte[8]; // version/flags, then entry count
        WriteUInt32(keyHeader, 4, 2);
        var keys = Box("keys", Concat(
            keyHeader,
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.creationdate")),
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.location.date"))));
        var ilst = Box("ilst", Concat(
            Box("\0\0\0\u0001", DataAtom(creationDate)),
            Box("\0\0\0\u0002", DataAtom(locationDate))));
        return Box("moov", Box("meta", Concat(keys, ilst)));
    }

    private static byte[] Box(string type, byte[] payload)
    {
        var size = 8 + payload.Length;
        var buffer = new byte[size];
        WriteUInt32(buffer, 0, (uint)size);
        buffer[4] = (byte)type[0];
        buffer[5] = (byte)type[1];
        buffer[6] = (byte)type[2];
        buffer[7] = (byte)type[3];
        Array.Copy(payload, 0, buffer, 8, payload.Length);
        return buffer;
    }

    /// <summary>Builds an iTunes-style "data" atom holding a UTF-8 string.</summary>
    private static byte[] DataAtom(string text)
    {
        var payload = Encoding.UTF8.GetBytes(text);
        var size = 16 + payload.Length;
        var buffer = new byte[size];
        WriteUInt32(buffer, 0, (uint)size);
        buffer[4] = (byte)'d';
        buffer[5] = (byte)'a';
        buffer[6] = (byte)'t';
        buffer[7] = (byte)'a';
        WriteUInt32(buffer, 8, 1);  // type indicator (UTF-8)
        WriteUInt32(buffer, 12, 0); // locale
        Array.Copy(payload, 0, buffer, 16, payload.Length);
        return buffer;
    }

    private static byte[] XtraStringEntry(string keyName, params string[] values)
    {
        var valueBlocks = new List<byte>();
        foreach (var value in values)
        {
            var data = Encoding.Unicode.GetBytes(value); // UTF-16LE
            var block = new byte[6 + data.Length];
            WriteUInt32(block, 0, (uint)(6 + data.Length)); // value size
            WriteUInt16(block, 4, 8);                       // value type (unicode string)
            Array.Copy(data, 0, block, 6, data.Length);
            valueBlocks.AddRange(block);
        }

        return XtraEntry(keyName, (uint)values.Length, valueBlocks.ToArray());
    }

    private static byte[] XtraRatingEntry(string keyName, long rating)
    {
        var block = new byte[14];
        WriteUInt32(block, 0, 10); // value length (ignored by reader)
        WriteUInt16(block, 4, 8);  // value type
        WriteInt64(block, 6, rating);
        return XtraEntry(keyName, 1, block);
    }

    private static byte[] XtraEntry(string keyName, uint entryCount, byte[] valueBlock)
    {
        var keyBytes = Encoding.ASCII.GetBytes(keyName);
        var entrySize = 4 + 4 + keyBytes.Length + 4 + valueBlock.Length;
        var buffer = new byte[entrySize];
        var offset = 0;
        WriteUInt32(buffer, offset, (uint)entrySize); offset += 4;
        WriteUInt32(buffer, offset, (uint)keyBytes.Length); offset += 4;
        Array.Copy(keyBytes, 0, buffer, offset, keyBytes.Length); offset += keyBytes.Length;
        WriteUInt32(buffer, offset, entryCount); offset += 4;
        Array.Copy(valueBlock, 0, buffer, offset, valueBlock.Length);
        return buffer;
    }

    private static byte[] Concat(params byte[][] arrays)
    {
        var result = new List<byte>();
        foreach (var array in arrays)
            result.AddRange(array);
        return result.ToArray();
    }

    private static void WriteUInt32(byte[] buffer, int offset, uint value)
    {
        buffer[offset] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)value;
    }

    private static void WriteUInt16(byte[] buffer, int offset, ushort value)
    {
        buffer[offset] = (byte)(value >> 8);
        buffer[offset + 1] = (byte)value;
    }

    private static void WriteInt64(byte[] buffer, int offset, long value)
    {
        for (var i = 0; i < 8; i++)
            buffer[offset + i] = (byte)(value >> (56 - 8 * i));
    }
}

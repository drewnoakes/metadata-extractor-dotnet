// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime;

public sealed class QuickTimeMetadataReaderTest
{
    [Theory]
    [CombinatorialData]
    public void ReadsMetadataWithOrWithoutFullBoxPrefix(
        bool inUserData, bool fullBox, bool keyed, bool extended, bool extendedHandler)
    {
        var handler = Box("hdlr", Concat(new byte[8], Encoding.ASCII.GetBytes(keyed ? "mdta" : "mdir")), extendedHandler);
        var keys = keyed ? Box("keys", Concat(new byte[4], [0, 0, 0, 2],
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.title")),
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.year"))), extended) : [];
        var ilst = Box("ilst", Concat(
            Box(keyed ? "\0\0\0\u0001" : "\u00a9nam", DataAtom("Title", extended), extended),
            Box(keyed ? "\0\0\0\u0002" : "yrrc", DataAtom("2024", extended), extended)), extended);
        var meta = Box("meta", Concat(fullBox ? new byte[4] : [], handler, keys, ilst), extended);
        var bytes = Box("moov", inUserData ? Box("udta", meta, extended) : meta, extended);
        using var stream = new MemoryStream(bytes);

        var directory = Assert.Single(QuickTimeMetadataReader.ReadMetadata(stream).OfType<QuickTimeMetadataHeaderDirectory>());

        Assert.Equal("Title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.Equal("2024", Assert.IsType<StringValue>(directory.GetObject(QuickTimeMetadataHeaderDirectory.TagYear)).ToString());
        Assert.False(directory.HasError);
        Assert.Equal(stream.Length, stream.Position);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsZeroSizedFirstMetadataChildWithOrWithoutFullBoxPrefix(bool fullBox, bool extended)
    {
        var ilst = Box("ilst", Box("\u00a9nam", DataAtom("Title")));
        WriteUInt32(ilst, 0, 0);
        var meta = Box("meta", Concat(fullBox ? new byte[4] : [], ilst), extended);
        using var stream = new MemoryStream(Box("moov", Box("udta", meta, extended), extended));

        var directory = Assert.Single(QuickTimeMetadataReader.ReadMetadata(stream).OfType<QuickTimeMetadataHeaderDirectory>());

        Assert.Equal("Title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void RejectsShortMetadataHandlerWithoutConsumingFollowingSiblings(
        [CombinatorialValues(0, 4, 8, 11)] int payloadLength, bool inUserData, bool extended)
    {
        var malformed = Box("hdlr", new byte[payloadLength], extended);
        var handler = Box("hdlr", Concat(new byte[8], Encoding.ASCII.GetBytes("mdir")), extended);
        var ilst = Box("ilst", Box("\u00a9nam", DataAtom("Following title", extended), extended), extended);
        var meta = Box("meta", Concat(inUserData ? new byte[4] : [], malformed, handler, ilst), extended);
        using var stream = new MemoryStream(Box("moov", inUserData ? Box("udta", meta, extended) : meta, extended));

        var directory = Assert.Single(QuickTimeMetadataReader.ReadMetadata(stream).OfType<QuickTimeMetadataHeaderDirectory>());

        Assert.Equal("Following title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.Equal("Invalid metadata handler atom size", Assert.Single(directory.Errors));
        Assert.Equal(stream.Length, stream.Position);
    }

    [Theory]
    [CombinatorialData]
    public void SkipsUnknownMetadataPayloadWithoutReadingIt(bool keyed, bool extended)
    {
        const int payloadLength = 128 * 1024;
        var headerSize = extended ? 16 : 8;
        var handler = Box("hdlr", Concat(new byte[8], Encoding.ASCII.GetBytes(keyed ? "mdta" : "mdir")), extended);
        var keys = keyed ? Box("keys", Concat(new byte[4], [0, 0, 0, 2],
            Box("mdta", Encoding.UTF8.GetBytes("com.example.unsupported")),
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.title"))), extended) : [];
        var prefix = Concat(new byte[4], handler, keys);
        var ignoredItem = Box(keyed ? "\0\0\0\u0001" : "covr", TypedDataAtom(13, new byte[payloadLength], extended), extended);
        var title = Box(keyed ? "\0\0\0\u0002" : "\u00a9nam", DataAtom("Following title", extended), extended);
        var bytes = Box("moov", Box("udta", Box("meta", Concat(prefix,
            Box("ilst", Concat(ignoredItem, title), extended)), extended), extended), extended);
        using var stream = new PayloadGuardStream(bytes, 6 * headerSize + prefix.Length + 8, payloadLength);

        var directory = Assert.Single(QuickTimeMetadataReader.ReadMetadata(stream).OfType<QuickTimeMetadataHeaderDirectory>());

        Assert.Equal("Following title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        if (keyed)
            Assert.Equal("Unsupported ilst key \"com.example.unsupported\"", Assert.Single(directory.Errors));
        else
            Assert.False(directory.HasError);
        Assert.Equal(stream.Length, stream.Position);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsItemListMetadataInBothContainers(bool inUserData, bool extended)
    {
        var handler = Box("hdlr", Concat(new byte[8], Encoding.ASCII.GetBytes("mdir")));
        var ilst = Box("ilst", Concat(
            Box("\u00a9alb", DataAtom("Album", extended), extended),
            Box("\u00a9ART", DataAtom("Artist", extended), extended),
            Box("@cmt", DataAtom("Comment", extended), extended),
            Box("desc", DataAtom("Description", extended), extended),
            Box("\u00a9nam", Concat(DataAtom("Earlier title"), DataAtom("Title", extended)), extended)), extended);
        var meta = Box("meta", Concat(inUserData ? new byte[4] : [], handler, ilst), extended);
        var bytes = Box("moov", inUserData ? Box("udta", meta, extended) : meta, extended);

        var directory = Assert.Single(QuickTimeMetadataReader.ReadMetadata(new MemoryStream(bytes))
            .OfType<QuickTimeMetadataHeaderDirectory>());

        Assert.Equal("Album", directory.GetString(QuickTimeMetadataHeaderDirectory.TagAlbum));
        Assert.Equal("Artist", directory.GetString(QuickTimeMetadataHeaderDirectory.TagArtist));
        Assert.Equal("Comment", directory.GetString(QuickTimeMetadataHeaderDirectory.TagComment));
        Assert.Equal("Description", directory.GetString(QuickTimeMetadataHeaderDirectory.TagDescription));
        Assert.Equal("Title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.False(directory.HasError);
    }

    [Fact]
    public void ReadsIndependentMetadataContainersWithoutLeakingKeysOrHandlers()
    {
        var itemList = Box("meta", Concat(
            Box("hdlr", Concat(new byte[8], Encoding.ASCII.GetBytes("mdir"))),
            Box("ilst", Box("\u00a9alb", DataAtom("Album")))));
        var title = KeyedMetadata("com.apple.quicktime.title", "Title", inUserData: false);
        var comment = KeyedMetadata("com.apple.quicktime.comment", "Comment", inUserData: true);
        var bytes = Box("moov", Concat(itemList, title, Box("udta", comment)));

        var directory = Assert.Single(QuickTimeMetadataReader.ReadMetadata(new MemoryStream(bytes))
            .OfType<QuickTimeMetadataHeaderDirectory>());

        Assert.Equal("Album", directory.GetString(QuickTimeMetadataHeaderDirectory.TagAlbum));
        Assert.Equal("Title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.Equal("Comment", directory.GetString(QuickTimeMetadataHeaderDirectory.TagComment));
        Assert.False(directory.HasError);

        static byte[] KeyedMetadata(string key, string value, bool inUserData)
        {
            return Box("meta", Concat(
                inUserData ? new byte[4] : [],
                Box("hdlr", Concat(new byte[8], Encoding.ASCII.GetBytes("mdta"))),
                Box("keys", Concat(new byte[4], [0, 0, 0, 1], Box("mdta", Encoding.UTF8.GetBytes(key)))),
                Box("ilst", Box("\0\0\0\u0001", DataAtom(value)))));
        }
    }

    [Theory, UseCulture("en-US")]
    [CombinatorialData]
    public void PreservesPartialOrUnparseableItemListDates(
        [CombinatorialValues("2024", "2024-05", "", "not a date", "2024-02-30", "12:34:56")] string text,
        bool inUserData, bool extended)
    {
        var directory = ReadItemListDate(text, inUserData, extended);

        var stored = Assert.IsType<StringValue>(directory.GetObject(QuickTimeMetadataHeaderDirectory.TagCreationDate));
        Assert.Equal(text, stored.ToString());
        Assert.Equal("Following title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsCompleteItemListDates(
        [CombinatorialValues("2024-05-06", "2024-05-06T12:34:56", "2024-05-06T12:34:56.1234567")] string text,
        bool inUserData, bool extended)
    {
        var directory = ReadItemListDate(text, inUserData, extended);
        var expected = new DateTime(2024, 5, 6);
        if (text.Length > 10)
            expected = expected.AddHours(12).AddMinutes(34).AddSeconds(56);
        if (text.Contains("."))
            expected = expected.AddTicks(1234567);

        var stored = Assert.IsType<DateTime>(directory.GetObject(QuickTimeMetadataHeaderDirectory.TagCreationDate));
        Assert.Equal(expected, stored);
        Assert.Equal(DateTimeKind.Unspecified, stored.Kind);
        Assert.Equal("Following title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsItemListDateOffsets(
        [CombinatorialValues("+0800", "-05:30", "Z")] string offset,
        bool inUserData, bool extended)
    {
        var directory = ReadItemListDate($"2024-05-06T12:34:56.1234567{offset}", inUserData, extended);
        var stored = Assert.IsType<DateTimeOffset>(directory.GetObject(QuickTimeMetadataHeaderDirectory.TagCreationDate));

        Assert.Equal(new DateTime(2024, 5, 6, 12, 34, 56).AddTicks(1234567), stored.DateTime);
        Assert.Equal(TimeSpan.FromMinutes(offset switch { "+0800" => 480, "-05:30" => -330, _ => 0 }), stored.Offset);
        Assert.Equal("Following title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.False(directory.HasError);
    }

    private static QuickTimeMetadataHeaderDirectory ReadItemListDate(string text, bool inUserData, bool extended)
    {
        var meta = Box("meta", Concat(
            inUserData ? new byte[4] : [],
            Box("hdlr", Concat(new byte[8], Encoding.ASCII.GetBytes("mdir"))),
            Box("ilst", Concat(
                Box("\u00a9day", DataAtom(text, extended), extended),
                Box("\u00a9nam", DataAtom("Following title", extended), extended)), extended)), extended);
        using var stream = new MemoryStream(Box("moov", inUserData ? Box("udta", meta, extended) : meta, extended));
        return Assert.Single(QuickTimeMetadataReader.ReadMetadata(stream).OfType<QuickTimeMetadataHeaderDirectory>());
    }

    [Fact]
    public void UnsupportedNumericItemListTagsDoNotOverwriteGenreOrUserRating()
    {
        var keyed = Box("meta", Concat(
            Box("keys", Concat(new byte[4], [0, 0, 0, 1],
                Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.rating.user")))),
            Box("ilst", Box("\0\0\0\u0001", TypedDataAtom(23, [0x40, 0x70, 0, 0], false)))));
        var itemList = Box("meta", Concat(new byte[4], Box("ilst", Concat(
            Box("\u00a9gen", DataAtom("Rock")),
            Box("gnre", TypedDataAtom(0, [0, 18], false)),
            Box("rtng", TypedDataAtom(21, [1], false)),
            Box("\u00a9nam", DataAtom("Following title"))))));
        using var stream = new MemoryStream(Box("moov", Concat(keyed, Box("udta", itemList))));

        var directory = Assert.Single(QuickTimeMetadataReader.ReadMetadata(stream).OfType<QuickTimeMetadataHeaderDirectory>());

        Assert.False(QuickTimeMetadataHeaderDirectory.TryGetTag("gnre", out _));
        Assert.False(QuickTimeMetadataHeaderDirectory.TryGetTag("rtng", out _));
        Assert.Equal("Rock", directory.GetString(QuickTimeMetadataHeaderDirectory.TagGenre));
        Assert.Equal(3.75f, directory.GetObject(QuickTimeMetadataHeaderDirectory.TagRating));
        Assert.Equal("Following title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.False(directory.HasError);
    }

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
    [CombinatorialData]
    public void ReadsKeyedDatesWithOffsetsWithoutLosingWallTime(
        [CombinatorialValues("+0800", "-05:30", "Z", "+08:00")] string offsetText,
        [CombinatorialValues("", ".1234567")] string fraction,
        bool extended)
    {
        var text = $"2025-04-11T20:01:03{fraction}{offsetText}";
        using var stream = new MemoryStream(KeyedDates(text, text, extended));
        var directory = QuickTimeMetadataReader.ReadMetadata(stream).OfType<QuickTimeMetadataHeaderDirectory>().Single();
        var wallTime = new DateTime(2025, 4, 11, 20, 1, 3).AddTicks(fraction.Length == 0 ? 0 : 1234567);
        var offsetMinutes = offsetText switch
        {
            "-05:30" => -330,
            "Z" => 0,
            _ => 480
        };
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

    private static byte[] KeyedDates(string creationDate, string locationDate, bool extended = false)
    {
        // moov/meta uses indexed keys, unlike the iTunes-style moov/udta/meta fixture.
        var keyHeader = new byte[8]; // version/flags, then entry count
        WriteUInt32(keyHeader, 4, 2);
        var keys = Box("keys", Concat(
            keyHeader,
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.creationdate")),
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.location.date"))), extended);
        var ilst = Box("ilst", Concat(
            Box("\0\0\0\u0001", DataAtom(creationDate, extended), extended),
            Box("\0\0\0\u0002", DataAtom(locationDate, extended), extended)), extended);
        return Box("moov", Box("meta", Concat(keys, ilst), extended), extended);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsMovieHeaderAndFollowingSiblings(bool extendedMoov, bool extendedMvhd)
    {
        var movie = new byte[100];
        WriteUInt32(movie, 4, 60); // created
        WriteUInt32(movie, 8, 120); // modified
        WriteUInt32(movie, 12, 1000); // time scale
        WriteUInt32(movie, 16, 2500); // duration
        WriteUInt32(movie, 20, 0x00010000); // preferred rate: 1
        WriteUInt16(movie, 24, 0x0100); // preferred volume: 1
        WriteUInt32(movie, 96, 7); // next track id

        var bytes = Concat(
            Box("moov", Concat(
                Box("free", [9, 8, 7], extended: true),
                Box("mvhd", movie, extendedMvhd),
                Box("udta", Box("meta", Concat(new byte[4],
                    Box("ilst", Box("\u00A9nam", DataAtom("After movie"))))))), extendedMoov),
            Box("ftyp", Concat(Encoding.ASCII.GetBytes("mp42"), [0, 0, 0, 1], Encoding.ASCII.GetBytes("isom"))));

        var directories = QuickTimeMetadataReader.ReadMetadata(new MemoryStream(bytes));
        var header = Assert.Single(directories.OfType<QuickTimeMovieHeaderDirectory>());
        Assert.Equal(1000, header.GetInt32(QuickTimeMovieHeaderDirectory.TagTimeScale));
        Assert.Equal(TimeSpan.FromMilliseconds(2500), header.GetObject(QuickTimeMovieHeaderDirectory.TagDuration));
        Assert.Equal(new DateTime(1904, 1, 1).AddSeconds(60), header.GetDateTime(QuickTimeMovieHeaderDirectory.TagCreated));
        Assert.Equal(7, header.GetInt32(QuickTimeMovieHeaderDirectory.TagNextTrackId));
        Assert.Equal("After movie", Assert.Single(directories.OfType<QuickTimeMetadataHeaderDirectory>())
            .GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.Equal("mp42", Assert.Single(directories.OfType<QuickTimeFileTypeDirectory>())
            .GetString(QuickTimeFileTypeDirectory.TagMajorBrand));
        Assert.All(directories, directory => Assert.False(directory.HasError));
    }

    [Theory]
    [CombinatorialData]
    public void ReadsMultipleItunesValuesWithoutSwallowingFollowingItem(bool extended)
    {
        var ilst = Box("ilst", Concat(
            Box("\u00A9nam", Concat(
                DataAtom("Earlier title", !extended),
                Box("free", [1, 2, 3], extended),
                DataAtom("Final title", extended)), extended),
            Box("\u00A9cmt", DataAtom("Following comment", !extended), !extended)), extended);
        var bytes = Box("moov", Box("udta", Box("meta", Concat(new byte[4], ilst), extended), extended), extended);

        var directory = Assert.Single(QuickTimeMetadataReader.ReadMetadata(new MemoryStream(bytes))
            .OfType<QuickTimeMetadataHeaderDirectory>());

        Assert.Equal("Final title", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.Equal("Following comment", directory.GetString(QuickTimeMetadataHeaderDirectory.TagComment));
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsItunesMetadataWithExtendedHeaders(
        bool extendedMoov, bool extendedUdta, bool extendedMeta, bool extendedIlst, bool extendedItem, bool extendedData)
    {
        var ilst = Box("ilst", Concat(
            Box("\u00A9nam", DataAtom("Titre café \u65E5\u672C", extendedData), extendedItem),
            Box("\u00A9cmt", DataAtom("Following comment", !extendedData), !extendedItem)), extendedIlst);
        var meta = Box("meta", Concat(new byte[4], ilst), extendedMeta);
        var bytes = Box("moov", Box("udta", meta, extendedUdta), extendedMoov);

        var directories = QuickTimeMetadataReader.ReadMetadata(new MemoryStream(bytes));

        var directory = Assert.Single(directories.OfType<QuickTimeMetadataHeaderDirectory>());
        Assert.Equal("Titre café \u65E5\u672C", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.Equal("Following comment", directory.GetString(QuickTimeMetadataHeaderDirectory.TagComment));
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsKeyedMetadataWithExtendedHeadersAndMultipleValues(
        bool extendedMoov, bool extendedMeta, bool extendedKeys, bool extendedIlst, bool extendedItem, bool extendedData)
    {
        var keys = Box("keys", Concat(new byte[4], [0, 0, 0, 3],
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.title")),
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.comment")),
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.rating.user"))), extendedKeys);
        var ilst = Box("ilst", Concat(
            Box("\0\0\0\u0001", Concat(
                DataAtom("Earlier value", !extendedData),
                DataAtom("Titre café \u65E5\u672C", extendedData)), extendedItem),
            Box("\0\0\0\u0002", DataAtom("Following keyed comment", !extendedData), !extendedItem),
            Box("\0\0\0\u0003", TypedDataAtom(23, [0x40, 0x70, 0, 0], extendedData), extendedItem)), extendedIlst);
        // Unlike udta/meta, this QuickTime metadata container has no FullBox prefix.
        var bytes = Box("moov", Box("meta", Concat(keys, ilst), extendedMeta), extendedMoov);

        var directories = QuickTimeMetadataReader.ReadMetadata(new MemoryStream(bytes));

        var directory = Assert.Single(directories.OfType<QuickTimeMetadataHeaderDirectory>());
        Assert.Equal("Titre café \u65E5\u672C", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.Equal("Following keyed comment", directory.GetString(QuickTimeMetadataHeaderDirectory.TagComment));
        Assert.Equal(3.75f, directory.GetObject(QuickTimeMetadataHeaderDirectory.TagRating));
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsKeyedBinaryValuesWithExtendedHeaders([CombinatorialValues(13, 14, 27)] int dataType)
    {
        byte[] artwork = [0x12, 0x34, 0x56, 0x78, 0x90];
        var keys = Box("keys", Concat(new byte[4], [0, 0, 0, 2],
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.artwork")),
            Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.title"))), extended: true);
        var ilst = Box("ilst", Concat(
            Box("\0\0\0\u0001", TypedDataAtom((uint)dataType, artwork, extended: true), extended: true),
            Box("\0\0\0\u0002", DataAtom("After artwork", extended: true), extended: true)), extended: true);
        var bytes = Box("moov", Box("meta", Concat(keys, ilst), extended: true), extended: true);

        var directory = Assert.Single(QuickTimeMetadataReader.ReadMetadata(new MemoryStream(bytes))
            .OfType<QuickTimeMetadataHeaderDirectory>());

        Assert.Equal(artwork, directory.GetByteArray(QuickTimeMetadataHeaderDirectory.TagArtwork));
        Assert.Equal("After artwork", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void ValidatesDataTypeAndLocaleHeaderWithoutSwallowingFollowingItem(
        bool keyed, [CombinatorialValues(0, 7, 8)] int payloadLength)
    {
        var payload = new byte[payloadLength];
        if (payloadLength >= 4)
            WriteUInt32(payload, 0, 1); // UTF-8, with no text when the payload is exactly eight bytes.
        var titleData = Box("data", payload, extended: true);
        byte[] bytes;
        if (keyed)
        {
            var keys = Box("keys", Concat(new byte[4], [0, 0, 0, 2],
                Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.title")),
                Box("mdta", Encoding.UTF8.GetBytes("com.apple.quicktime.comment"))), extended: true);
            var ilst = Box("ilst", Concat(
                Box("\0\0\0\u0001", titleData, extended: true),
                Box("\0\0\0\u0002", DataAtom("After data", extended: true), extended: true)), extended: true);
            bytes = Box("moov", Box("meta", Concat(keys, ilst), extended: true), extended: true);
        }
        else
        {
            var ilst = Box("ilst", Concat(
                Box("\u00A9nam", titleData, extended: true),
                Box("\u00A9cmt", DataAtom("After data", extended: true), extended: true)), extended: true);
            bytes = Box("moov", Box("udta", Box("meta", Concat(new byte[4], ilst), extended: true), extended: true), extended: true);
        }

        var directory = Assert.Single(QuickTimeMetadataReader.ReadMetadata(new MemoryStream(bytes))
            .OfType<QuickTimeMetadataHeaderDirectory>());

        Assert.Equal("After data", directory.GetString(QuickTimeMetadataHeaderDirectory.TagComment));
        if (payloadLength < 8)
        {
            Assert.False(directory.ContainsTag(QuickTimeMetadataHeaderDirectory.TagTitle));
            Assert.Contains("data atom size", Assert.Single(directory.Errors));
        }
        else
        {
            Assert.Equal("", directory.GetString(QuickTimeMetadataHeaderDirectory.TagTitle));
            Assert.False(directory.HasError);
        }
    }

    private sealed class PayloadGuardStream(byte[] bytes, long payloadStart, long payloadLength) : MemoryStream(bytes)
    {
        public override int Read(byte[] buffer, int offset, int count)
        {
            Assert.False(Position < payloadStart + payloadLength && Position + count > payloadStart,
                "The unsupported metadata payload must be skipped, not read.");
            return base.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            Assert.False(Position >= payloadStart && Position < payloadStart + payloadLength,
                "The unsupported metadata payload must be skipped, not read.");
            return base.ReadByte();
        }
    }

    private static byte[] Box(string type, byte[] payload, bool extended = false)
    {
        var headerSize = extended ? 16 : 8;
        var size = headerSize + payload.Length;
        var buffer = new byte[size];
        WriteUInt32(buffer, 0, extended ? 1U : (uint)size);
        buffer[4] = (byte)type[0];
        buffer[5] = (byte)type[1];
        buffer[6] = (byte)type[2];
        buffer[7] = (byte)type[3];
        if (extended)
            WriteInt64(buffer, 8, size);
        Array.Copy(payload, 0, buffer, headerSize, payload.Length);
        return buffer;
    }

    /// <summary>Builds an iTunes-style "data" atom holding a UTF-8 string.</summary>
    private static byte[] DataAtom(string text, bool extended = false)
    {
        return TypedDataAtom(1, Encoding.UTF8.GetBytes(text), extended);
    }

    private static byte[] TypedDataAtom(uint type, byte[] value, bool extended)
    {
        var payload = new byte[8 + value.Length];
        WriteUInt32(payload, 0, type);
        // Four-byte locale stays zero.
        Array.Copy(value, 0, payload, 8, value.Length);
        return Box("data", payload, extended);
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

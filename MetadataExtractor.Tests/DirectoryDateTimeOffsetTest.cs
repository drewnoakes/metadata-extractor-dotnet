// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Globalization;

namespace MetadataExtractor;

public sealed class DirectoryDateTimeOffsetTest
{
    public static IEnumerable<object> ExplicitOffsetDates()
    {
        (string Text, int OffsetMinutes, int Second, int FractionTicks)[] cases =
        [
            ("2025-04-11T20:01:03+0800", 480, 3, 0),
            ("2025-04-11T20:01:03+08:00", 480, 3, 0),
            ("2025-04-11T20:01:03-05:30", -330, 3, 0),
            ("2025-04-11T20:01:03-0530", -330, 3, 0),
            ("2025-04-11T20:01:03+08", 480, 3, 0),
            ("2025-04-11T20:01:03-05", -300, 3, 0),
            ("2025-04-11T20:01:03Z", 0, 3, 0),
            ("2025-04-11T20:01:03z", 0, 3, 0),
            ("2025-04-11T20:01:03+00:00", 0, 3, 0),
            ("2025-04-11T20:01:03.1+08:00", 480, 3, 1000000),
            ("2025-04-11T20:01:03.12-0530", -330, 3, 1200000),
            ("2025-04-11T20:01:03.123Z", 0, 3, 1230000),
            ("2025-04-11T20:01:03.1234+0800", 480, 3, 1234000),
            ("2025-04-11T20:01:03.12345+08:00", 480, 3, 1234500),
            ("2025-04-11T20:01:03.123456-05", -300, 3, 1234560),
            ("2025-04-11T20:01:03.1234567z", 0, 3, 1234567),
            ("2025:04:11 20:01:03+08:00", 480, 3, 0),
            ("2025:04:11 20:01:03.123-05:30", -330, 3, 1230000),
            ("2025:04:11 20:01+08:00", 480, 0, 0),
            ("2025-04-11 20:01:03+08:00", 480, 3, 0),
            ("2025-04-11 20:01:03.123-05:30", -330, 3, 1230000),
            ("2025-04-11 20:01+08:00", 480, 0, 0),
            ("2025.04.11 20:01:03+08:00", 480, 3, 0),
            ("2025.04.11 20:01-05:30", -330, 0, 0),
            ("2025-04-11T20:01+08:00", 480, 0, 0)
        ];

        foreach (var item in cases)
            yield return new object[] { item.Text, item.OffsetMinutes, item.Second, item.FractionTicks };
    }

    [Theory]
    [CombinatorialData]
    public void GetDateTimeOffset_ExplicitOffsetText_PreservesWallTimeOffsetAndInstant(
        [CombinatorialMemberData(nameof(ExplicitOffsetDates))] object[] date,
        bool stringValue,
        [CombinatorialValues("en-US", "ar-SA")] string culture)
    {
        var text = (string)date[0];
        var offsetMinutes = (int)date[1];
        var second = (int)date[2];
        var fractionTicks = (int)date[3];
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            var directory = new MockDirectory();
            directory.Set(1, stringValue ? (object)new StringValue(Encoding.UTF8.GetBytes(text), Encoding.UTF8) : text);
            var wallTime = new DateTime(2025, 4, 11, 20, 1, second).AddTicks(fractionTicks);
            var offset = TimeSpan.FromMinutes(offsetMinutes);

            Assert.True(directory.TryGetDateTimeOffset(1, out var actual));
            Assert.Equal(wallTime, actual.DateTime);
            Assert.Equal(offset, actual.Offset);
            Assert.Equal(DateTime.SpecifyKind(wallTime - offset, DateTimeKind.Utc), actual.UtcDateTime);
            Assert.True(actual.EqualsExact(directory.GetDateTimeOffset(1)));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    public static IEnumerable<string> UnknownOrInvalidOffsetDates()
    {
        return
        [
            "",
            "not a date",
            "    :  :     :  :  ",
            "2025",
            "2025-04",
            "20250411",
            "2025-04-11",
            "2025:04:11",
            "2025-04-11T20:01",
            "2025-04-11T20:01:03",
            "2025-04-11T20:01:03.1234567",
            "2025:04:11 20:01:03",
            "2025-04-11 20:01:03",
            "2025.04.11 20:01:03",
            "2025-04-11T20:01:03+15:00",
            "2025-04-11T20:01:03+08:60",
            "2025-04-11T20:01:03.12345678+08:00",
            "2025-02-30T20:01:03+08:00",
            "2025-04-11T24:01:03Z",
            "04/11/2025 20:01:03 +08:00"
        ];

    }

    [Theory]
    [CombinatorialData]
    public void GetDateTimeOffset_UnknownOrInvalidOffsetText_RejectsValue(
        [CombinatorialMemberData(nameof(UnknownOrInvalidOffsetDates))] string text,
        bool stringValue)
    {
        var directory = new MockDirectory();
        directory.Set(1, stringValue ? (object)new StringValue(Encoding.UTF8.GetBytes(text), Encoding.UTF8) : text);

        Assert.False(directory.TryGetDateTimeOffset(1, out var actual));
        Assert.Equal(default(DateTimeOffset), actual);
        Assert.Throws<MetadataException>(() => directory.GetDateTimeOffset(1));
    }

    [Theory]
    [CombinatorialData]
    public void GetDateTimeOffset_StoredDateTimeOffset_PreservesOriginalOffset(
        [CombinatorialValues(480, -330, 0)] int offsetMinutes)
    {
        var directory = new MockDirectory();
        var wallTime = new DateTime(2025, 4, 11, 20, 1, 3).AddTicks(1234567);
        directory.Set(1, new DateTimeOffset(wallTime, TimeSpan.FromMinutes(offsetMinutes)));

        Assert.True(directory.TryGetDateTimeOffset(1, out var actual));
        Assert.Equal(wallTime, actual.DateTime);
        Assert.Equal(TimeSpan.FromMinutes(offsetMinutes), actual.Offset);
        Assert.True(actual.EqualsExact(directory.GetDateTimeOffset(1)));
    }

    [Fact]
    public void GetDateTimeOffset_StoredUtcDateTime_UsesZeroOffset()
    {
        var directory = new MockDirectory();
        directory.Set(1, new DateTime(2025, 4, 11, 12, 1, 3, DateTimeKind.Utc).AddTicks(1234567));

        Assert.True(directory.TryGetDateTimeOffset(1, out var actual));
        Assert.Equal(TimeSpan.Zero, actual.Offset);
        Assert.Equal(new DateTime(2025, 4, 11, 12, 1, 3).AddTicks(1234567), actual.DateTime);
        Assert.Equal(DateTimeKind.Utc, actual.UtcDateTime.Kind);
        Assert.True(actual.EqualsExact(directory.GetDateTimeOffset(1)));
    }

    [Theory]
    [CombinatorialData]
    public void GetDateTimeOffset_StoredDateTimeWithoutKnownOffset_RejectsValue(
        [CombinatorialValues(DateTimeKind.Local, DateTimeKind.Unspecified)] DateTimeKind kind)
    {
        var directory = new MockDirectory();
        directory.Set(1, new DateTime(2025, 4, 11, 20, 1, 3, kind));

        Assert.False(directory.TryGetDateTimeOffset(1, out var actual));
        Assert.Equal(default(DateTimeOffset), actual);
        Assert.Throws<MetadataException>(() => directory.GetDateTimeOffset(1));
    }

    [Fact]
    public void GetDateTimeOffset_MissingTag_ReturnsFalseAndGetterThrows()
    {
        var directory = new MockDirectory();

        Assert.False(directory.TryGetDateTimeOffset(1, out var actual));
        Assert.Equal(default(DateTimeOffset), actual);
        Assert.Throws<MetadataException>(() => directory.GetDateTimeOffset(1));
    }

    [Theory]
    [CombinatorialData]
    public void GetDateTimeOffset_UnsupportedStoredValue_RejectsValue(
        [CombinatorialValues(123, true)] object value)
    {
        var directory = new MockDirectory();
        directory.Set(1, value);

        Assert.False(directory.TryGetDateTimeOffset(1, out var actual));
        Assert.Equal(default(DateTimeOffset), actual);
        Assert.Throws<MetadataException>(() => directory.GetDateTimeOffset(1));
    }

    [Theory]
    [CombinatorialData]
    public void GetDateTime_StoredDateTimeOffset_ConvertsToLocalTimeWithoutChangingInstant(
        [CombinatorialValues(480, -330, 0)] int offsetMinutes)
    {
        var directory = new MockDirectory();
        var value = new DateTimeOffset(2025, 4, 11, 20, 1, 3, TimeSpan.FromMinutes(offsetMinutes)).AddTicks(1234567);
        directory.Set(1, value);

        Assert.True(directory.TryGetDateTime(1, out var actual));
        Assert.Equal(DateTimeKind.Local, actual.Kind);
        Assert.Equal(value.LocalDateTime, actual);
        Assert.Equal(value.UtcDateTime, actual.ToUniversalTime());
        Assert.Equal(actual, directory.GetDateTime(1));
        Assert.Equal(DateTimeKind.Local, directory.GetDateTime(1).Kind);
    }

    [Theory]
    [CombinatorialData]
    public void GetDateTime_OffsetString_KeepsExistingUtcConversion(bool stringValue)
    {
        const string text = "2002-01-30T23:59:59.099-08:00";
        var directory = new MockDirectory();
        directory.Set(1, stringValue ? (object)new StringValue(Encoding.UTF8.GetBytes(text), Encoding.UTF8) : text);

        var actual = directory.GetDateTime(1);

        Assert.Equal(new DateTime(2002, 1, 31, 7, 59, 59, 99, DateTimeKind.Utc), actual);
        Assert.Equal(DateTimeKind.Utc, actual.Kind);
    }
}

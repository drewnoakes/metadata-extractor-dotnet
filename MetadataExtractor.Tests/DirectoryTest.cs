// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Globalization;
using MetadataExtractor.Formats.Exif;

namespace MetadataExtractor.Tests
{
    /// <summary>Unit tests for <see cref="Directory"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
    public sealed class DirectoryTest
    {
        private readonly Directory _directory;

        // TODO write tests to validate type conversions from all underlying types

        public DirectoryTest()
        {
            _directory = new MockDirectory();
        }

        [Fact]
        public void SetAndGetMultipleTagsInSingleDirectory()
        {
            _directory.Set(ExifDirectoryBase.TagAperture, "TAG_APERTURE");
            _directory.Set(ExifDirectoryBase.TagBatteryLevel, "TAG_BATTERY_LEVEL");
            Assert.Equal("TAG_APERTURE", _directory.GetString(ExifDirectoryBase.TagAperture));
            Assert.Equal("TAG_BATTERY_LEVEL", _directory.GetString(ExifDirectoryBase.TagBatteryLevel));
        }

        [Fact]
        public void SetSameTagMultipleTimesOverwritesValue()
        {
            _directory.Set(ExifDirectoryBase.TagAperture, 1);
            _directory.Set(ExifDirectoryBase.TagAperture, 2);
            Assert.Equal(2, _directory.GetInt32(ExifDirectoryBase.TagAperture));
        }

        [Fact]
        public void UnderlyingInt()
        {
            var value = 123;
            var tagType = 321;
            _directory.Set(tagType, value);
            Assert.Equal(value, _directory.GetInt32(tagType));
            Assert.Equal(value, _directory.GetSingle(tagType), precision: 5);
            Assert.Equal(value, _directory.GetDouble(tagType), precision: 5);
            Assert.Equal((long)value, (object)_directory.GetInt64(tagType));
            Assert.Equal(value.ToString(), _directory.GetString(tagType));
            Assert.True(_directory.TryGetRational(tagType, out Rational rational));
            Assert.Equal(new Rational(value, 1), rational);
            Assert.Equal([value], _directory.GetInt32Array(tagType));
            Assert.Equal(new[] { unchecked((byte)value) }, _directory.GetByteArray(tagType));
        }

        [Fact]
        public void SetAndGetIntArray()
        {
            var inputValues = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var tagType = 123;
            _directory.Set(tagType, inputValues);
            var outputValues = _directory.GetInt32Array(tagType);
            Assert.NotNull(outputValues);
            Assert.Equal(inputValues.Length, outputValues!.Length);
            for (var i = 0; i < inputValues.Length; i++)
            {
                var inputValue = inputValues[i];
                var outputValue = outputValues[i];
                Assert.Equal(inputValue, outputValue);
            }
            Assert.Equal(inputValues, _directory.GetInt32Array(tagType));
            var outputString = new StringBuilder();
            for (var i1 = 0; i1 < inputValues.Length; i1++)
            {
                var inputValue = inputValues[i1];
                if (i1 > 0)
                {
                    outputString.Append(' ');
                }
                outputString.Append(inputValue);
            }
            Assert.Equal(outputString.ToString(), _directory.GetString(tagType));
        }

        [Theory]
#pragma warning disable format
        [InlineData("2002:01:30 23:59:59",           2002, 1, 30, 23, 59, 59,  0, DateTimeKind.Unspecified)]
        [InlineData("2002:01:30 23:59",              2002, 1, 30, 23, 59,  0,  0, DateTimeKind.Unspecified)]
        [InlineData("2002-01-30 23:59:59",           2002, 1, 30, 23, 59, 59,  0, DateTimeKind.Unspecified)]
        [InlineData("2002-01-30 23:59",              2002, 1, 30, 23, 59,  0,  0, DateTimeKind.Unspecified)]
        [InlineData("2002-01-30T23:59:59.099-08:00", 2002, 1, 31,  7, 59, 59, 99, DateTimeKind.Utc)]
        [InlineData("2002-01-30T23:59:59.9",         2002, 1, 30, 23, 59, 59,900, DateTimeKind.Unspecified)]
        [InlineData("2002-01-30T23:59:59.09",        2002, 1, 30, 23, 59, 59, 90, DateTimeKind.Unspecified)]
        [InlineData("2002-01-30T23:59:59.099",       2002, 1, 30, 23, 59, 59, 99, DateTimeKind.Unspecified)]
        [InlineData("2002-01-30T23:59:59-08:00",     2002, 1, 31,  7, 59, 59,  0, DateTimeKind.Utc)]
        [InlineData("2002-01-30T23:59:59",           2002, 1, 30, 23, 59, 59,  0, DateTimeKind.Unspecified)]
        [InlineData("2002-01-30T23:59:59+0100",      2002, 1, 30, 22, 59, 59,  0, DateTimeKind.Utc)]
        [InlineData("2002-01-30T23:59-08:00",        2002, 1, 31,  7, 59,  0,  0, DateTimeKind.Utc)]
        [InlineData("2002-01-30T23:59",              2002, 1, 30, 23, 59,  0,  0, DateTimeKind.Unspecified)]
        [InlineData("2002-01-30",                    2002, 1, 30,  0,  0,  0,  0, DateTimeKind.Unspecified)]
        [InlineData("2002-01",                       2002, 1,  1,  0,  0,  0,  0, DateTimeKind.Unspecified)]
        [InlineData("2002",                          2002, 1,  1,  0,  0,  0,  0, DateTimeKind.Unspecified)]
        [InlineData("2002-01-30T23:59:59.099-08:00", 2002, 1, 31,  7, 59, 59, 99, DateTimeKind.Utc, "ar")]
#pragma warning restore format
        public void SetStringAndGetDate(string str, int year, int month, int day, int hour, int minute, int second, int milli, DateTimeKind kind, string? culture = null)
        {
            CultureInfo.CurrentCulture = string.IsNullOrWhiteSpace(culture) ? CultureInfo.CurrentCulture : CultureInfo.GetCultureInfo(culture);

            _directory.Set(1, str);

            var expected = new DateTime(year, month, day, hour, minute, second, milli, kind);
            var actual = _directory.GetDateTime(1);

            Assert.Equal(expected.Kind, actual.Kind);
            Assert.Equal(expected, actual);

            // TODO revisit these commented cases and introduce GetDateTimeOffset impl/test
        }

        [Fact]
        public void SetIntArrayGetByteArray()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            _directory.Set(1, ints);
            var bytes = _directory.GetByteArray(1);
            Assert.NotNull(bytes);
            Assert.Equal(ints.Length, bytes!.Length);
            Assert.Equal(1, bytes[0]);
        }

        [Fact(Skip = "TODO test whether this is needed in a regression test against the image database")]
        public void SetStringGetInt()
        {
            var bytes = new byte[] { 0x01, 0x02, 0x03 };
            _directory.Set(1, Encoding.UTF8.GetString(bytes));
            Assert.Equal(0x010203, _directory.GetInt32(1));
        }

        [Fact]
        public void ContainsTag()
        {
            Assert.False(_directory.ContainsTag(ExifDirectoryBase.TagAperture));
            _directory.Set(ExifDirectoryBase.TagAperture, "Tag Value");
            Assert.True(_directory.ContainsTag(ExifDirectoryBase.TagAperture));
        }

        [Fact]
        public void GetNonExistentTagIsNullForAllTypes()
        {
            Assert.Null(_directory.GetObject(ExifDirectoryBase.TagAperture));

            Assert.Null(_directory.GetString(ExifDirectoryBase.TagAperture));

            Assert.Null(_directory.GetByteArray(ExifDirectoryBase.TagAperture));
            Assert.Null(_directory.GetInt32Array(ExifDirectoryBase.TagAperture));
            Assert.Null(_directory.GetRationalArray(ExifDirectoryBase.TagAperture));
            Assert.Null(_directory.GetStringArray(ExifDirectoryBase.TagAperture));

            Assert.False(_directory.TryGetBoolean(ExifDirectoryBase.TagAperture, out _));
            Assert.False(_directory.TryGetDateTime(ExifDirectoryBase.TagAperture, out _));
            Assert.False(_directory.TryGetDouble(ExifDirectoryBase.TagAperture, out _));
            Assert.False(_directory.TryGetInt32(ExifDirectoryBase.TagAperture, out _));
            Assert.False(_directory.TryGetInt64(ExifDirectoryBase.TagAperture, out _));
            Assert.False(_directory.TryGetRational(ExifDirectoryBase.TagAperture, out _));
            Assert.False(_directory.TryGetSingle(ExifDirectoryBase.TagAperture, out _));
        }

        [Fact]
        public void ToStringMethod()
        {
            var directory = new ExifIfd0Directory();
            Assert.Equal("Exif IFD0 Directory (0 tags)", directory.ToString());
            directory.Set(1, "Tag 1");
            Assert.Equal("Exif IFD0 Directory (1 tag)", directory.ToString());
            directory.Set(2, "Tag 2");
            Assert.Equal("Exif IFD0 Directory (2 tags)", directory.ToString());
        }
    }
}

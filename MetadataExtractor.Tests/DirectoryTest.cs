#region License
//
// Copyright 2002-2016 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MetadataExtractor.Formats.Exif;
using Xunit;

namespace MetadataExtractor.Tests
{
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
        public void TestSetAndGetMultipleTagsInSingleDirectory()
        {
            _directory.Set(ExifDirectoryBase.TagAperture, "TAG_APERTURE");
            _directory.Set(ExifDirectoryBase.TagBatteryLevel, "TAG_BATTERY_LEVEL");
            Assert.Equal("TAG_APERTURE", _directory.GetString(ExifDirectoryBase.TagAperture));
            Assert.Equal("TAG_BATTERY_LEVEL", _directory.GetString(ExifDirectoryBase.TagBatteryLevel));
        }


        [Fact]
        public void TestSetSameTagMultipleTimesOverwritesValue()
        {
            _directory.Set(ExifDirectoryBase.TagAperture, 1);
            _directory.Set(ExifDirectoryBase.TagAperture, 2);
            Assert.Equal(2, _directory.GetInt32(ExifDirectoryBase.TagAperture));
        }


        [Fact]
        public void TestUnderlyingInt()
        {
            var value = 123;
            var tagType = 321;
            _directory.Set(tagType, value);
            Assert.Equal(value, _directory.GetInt32(tagType));
            Assert.Equal(value, _directory.GetSingle(tagType), precision: 5);
            Assert.Equal(value, _directory.GetDouble(tagType), precision: 5);
            Assert.Equal((long)value, (object)_directory.GetInt64(tagType));
            Assert.Equal(value.ToString(), _directory.GetString(tagType));
            Rational rational;
            Assert.True(_directory.TryGetRational(tagType, out rational));
            Assert.Equal(new Rational(value, 1), rational);
            Assert.Equal(new[] { value }, _directory.GetInt32Array(tagType));
            Assert.Equal(new[] { unchecked((byte)value) }, _directory.GetByteArray(tagType));
        }


        [Fact]
        public void TestSetAndGetIntArray()
        {
            var inputValues = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var tagType = 123;
            _directory.Set(tagType, inputValues);
            var outputValues = _directory.GetInt32Array(tagType);
            Assert.NotNull(outputValues);
            Assert.Equal(inputValues.Length, outputValues.Length);
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


        [Fact]
        public void TestSetStringAndGetDate()
        {
            Action<string, DateTimeOffset> test = (str, expected) =>
            {
                _directory.Set(1, str);
                Assert.Equal(expected, _directory.GetDateTimeOffset(1));
            };

            test("2002:01:30 23:59:59",           new DateTimeOffset(2002, 1, 30, 23, 59, 59,     new TimeSpan(0)));
            test("2002:01:30 23:59",              new DateTimeOffset(2002, 1, 30, 23, 59,  0,     new TimeSpan(0)));
            test("2002-01-30 23:59:59",           new DateTimeOffset(2002, 1, 30, 23, 59, 59,     new TimeSpan(0)));
            test("2002-01-30 23:59",              new DateTimeOffset(2002, 1, 30, 23, 59,  0,     new TimeSpan(0)));
            test("2002-01-30T23:59:59.099-08:00", new DateTimeOffset(2002, 1, 30, 23, 59, 59, 99, new TimeSpan(-8, 0, 0)));
            test("2002-01-30T23:59:59.099",       new DateTimeOffset(2002, 1, 30, 23, 59, 59, 99, new TimeSpan(0)));
            test("2002-01-30T23:59:59-08:00",     new DateTimeOffset(2002, 1, 30, 23, 59, 59,     new TimeSpan(-8, 0, 0)));
            test("2002-01-30T23:59:59",           new DateTimeOffset(2002, 1, 30, 23, 59, 59,     new TimeSpan(0)));
            test("2002-01-30T23:59-08:00",        new DateTimeOffset(2002, 1, 30, 23, 59,  0,     new TimeSpan(-8, 0, 0)));
            test("2002-01-30T23:59",              new DateTimeOffset(2002, 1, 30, 23, 59,  0,     new TimeSpan(0)));
            test("2002-01-30",                    new DateTimeOffset(2002, 1, 30,  0,  0,  0,     new TimeSpan(0)));
            test("2002-01",                       new DateTimeOffset(2002, 1,  1,  0,  0,  0,     new TimeSpan(0)));
            test("2002",                          new DateTimeOffset(2002, 1,  1,  0,  0,  0,     new TimeSpan(0)));
        }


        [Fact]
        public void TestSetIntArrayGetByteArray()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            _directory.Set(1, ints);
            var bytes = _directory.GetByteArray(1);
            Assert.NotNull(bytes);
            Assert.Equal(ints.Length, bytes.Length);
            Assert.Equal(1, bytes[0]);
        }


        [Fact(Skip = "TODO test whether this is needed in a regression test against the image database")]
        public void TestSetStringGetInt()
        {
            var bytes = new byte[] { 0x01, 0x02, 0x03 };
            _directory.Set(1, Encoding.UTF8.GetString(bytes));
            Assert.Equal(0x010203, _directory.GetInt32(1));
        }


        [Fact]
        public void TestContainsTag()
        {
            Assert.False(_directory.ContainsTag(ExifDirectoryBase.TagAperture));
            _directory.Set(ExifDirectoryBase.TagAperture, "Tag Value");
            Assert.True(_directory.ContainsTag(ExifDirectoryBase.TagAperture));
        }


        [Fact]
        public void TestGetNonExistentTagIsNullForAllTypes()
        {
            Assert.Null(_directory.GetObject(ExifDirectoryBase.TagAperture));

            Assert.Null(_directory.GetString(ExifDirectoryBase.TagAperture));

            Assert.Null(_directory.GetByteArray(ExifDirectoryBase.TagAperture));
            Assert.Null(_directory.GetInt32Array(ExifDirectoryBase.TagAperture));
            Assert.Null(_directory.GetRationalArray(ExifDirectoryBase.TagAperture));
            Assert.Null(_directory.GetStringArray(ExifDirectoryBase.TagAperture));

            bool b;
            Assert.False(_directory.TryGetBoolean(ExifDirectoryBase.TagAperture, out b));
            DateTimeOffset dt;
            Assert.False(_directory.TryGetDateTimeOffset(ExifDirectoryBase.TagAperture, out dt));
            double d;
            Assert.False(_directory.TryGetDouble(ExifDirectoryBase.TagAperture, out d));
            int i;
            Assert.False(_directory.TryGetInt32(ExifDirectoryBase.TagAperture, out i));
            long l;
            Assert.False(_directory.TryGetInt64(ExifDirectoryBase.TagAperture, out l));
            Rational r;
            Assert.False(_directory.TryGetRational(ExifDirectoryBase.TagAperture, out r));
            float f;
            Assert.False(_directory.TryGetSingle(ExifDirectoryBase.TagAperture, out f));
        }

        [Fact]
        public void TestToString()
        {
            Directory directory = new ExifIfd0Directory();
            Assert.Equal("Exif IFD0 Directory (0 tags)", directory.ToString());
            directory.Set(1, "Tag 1");
            Assert.Equal("Exif IFD0 Directory (1 tag)", directory.ToString());
            directory.Set(2, "Tag 2");
            Assert.Equal("Exif IFD0 Directory (2 tags)", directory.ToString());
        }
    }
}

/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using System.Text;
using MetadataExtractor.Formats.Exif;
using NUnit.Framework;

namespace MetadataExtractor.Tests
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class DirectoryTest
    {
        private Directory _directory;

        // TODO write tests to validate type conversions from all underlying types
        [SetUp]
        public void Setup()
        {
            _directory = new MockDirectory();
        }


        [Test]
        public void TestSetAndGetMultipleTagsInSingleDirectory()
        {
            _directory.Set(ExifDirectoryBase.TagAperture, "TAG_APERTURE");
            _directory.Set(ExifDirectoryBase.TagBatteryLevel, "TAG_BATTERY_LEVEL");
            Assert.AreEqual("TAG_APERTURE", _directory.GetString(ExifDirectoryBase.TagAperture));
            Assert.AreEqual("TAG_BATTERY_LEVEL", _directory.GetString(ExifDirectoryBase.TagBatteryLevel));
        }


        [Test]
        public void TestSetSameTagMultipleTimesOverwritesValue()
        {
            _directory.Set(ExifDirectoryBase.TagAperture, 1);
            _directory.Set(ExifDirectoryBase.TagAperture, 2);
            Assert.AreEqual(2, _directory.GetInt32(ExifDirectoryBase.TagAperture));
        }


        [Test]
        public void TestUnderlyingInt()
        {
            var value = 123;
            var tagType = 321;
            _directory.Set(tagType, value);
            Assert.AreEqual(value, _directory.GetInt32(tagType));
            Assert.AreEqual(value, _directory.GetInt32Nullable(tagType));
            Assert.AreEqual(value, _directory.GetSingle(tagType), 0.00001);
            Assert.AreEqual(value, _directory.GetDouble(tagType), 0.00001);
            Assert.AreEqual((long)value, (object)_directory.GetInt64(tagType));
            Assert.AreEqual(value.ToString(), _directory.GetString(tagType));
            Assert.AreEqual(new Rational(value, 1), _directory.GetRational(tagType));
            CollectionAssert.AreEqual(new[] { value }, _directory.GetInt32Array(tagType));
            CollectionAssert.AreEqual(new[] { unchecked((byte)value) }, _directory.GetByteArray(tagType));
        }


        [Test]
        public void TestSetAndGetIntArray()
        {
            var inputValues = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var tagType = 123;
            _directory.Set(tagType, inputValues);
            var outputValues = _directory.GetInt32Array(tagType);
            Assert.IsNotNull(outputValues);
            Assert.AreEqual(inputValues.Length, outputValues.Length);
            for (var i = 0; i < inputValues.Length; i++)
            {
                var inputValue = inputValues[i];
                var outputValue = outputValues[i];
                Assert.AreEqual(inputValue, outputValue);
            }
            CollectionAssert.AreEqual(inputValues, _directory.GetInt32Array(tagType));
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
            Assert.AreEqual(outputString.ToString(), _directory.GetString(tagType));
        }


        [Test]
        public void TestSetStringAndGetDate()
        {
            var date1 = "2002:01:30 23:59:59";
            var date2 = "2002:01:30 23:59";
            var date3 = "2002-01-30 23:59:59";
            var date4 = "2002-01-30 23:59";
            _directory.Set(1, date1);
            _directory.Set(2, date2);
            _directory.Set(3, date3);
            _directory.Set(4, date4);
            Assert.AreEqual(date1, _directory.GetString(1));
            Assert.AreEqual(new DateTime(2002, 1, 30, 23, 59, 59), _directory.GetDateTimeNullable(1));
            Assert.AreEqual(new DateTime(2002, 1, 30, 23, 59, 0), _directory.GetDateTimeNullable(2));
            Assert.AreEqual(new DateTime(2002, 1, 30, 23, 59, 59), _directory.GetDateTimeNullable(3));
            Assert.AreEqual(new DateTime(2002, 1, 30, 23, 59, 0), _directory.GetDateTimeNullable(4));
        }


        [Test]
        public void TestSetIntArrayGetByteArray()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            _directory.Set(1, ints);
            var bytes = _directory.GetByteArray(1);
            Assert.IsNotNull(bytes);
            Assert.AreEqual(ints.Length, bytes.Length);
            Assert.AreEqual(1, bytes[0]);
        }


        [Test]
        public void TestSetStringGetInt()
        {
            var bytes = new byte[] { 0x01, 0x02, 0x03 };
            _directory.Set(1, Encoding.UTF8.GetString(bytes));
            Assert.AreEqual(0x010203, _directory.GetInt32(1));
        }


        [Test]
        public void TestContainsTag()
        {
            Assert.IsFalse(_directory.ContainsTag(ExifDirectoryBase.TagAperture));
            _directory.Set(ExifDirectoryBase.TagAperture, "Tag Value");
            Assert.IsTrue(_directory.ContainsTag(ExifDirectoryBase.TagAperture));
        }


        [Test]
        public void TestGetNonExistentTagIsNullForAllTypes()
        {
            Assert.IsNull(_directory.GetString(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetInt32Nullable(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetDoubleNullable(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetSingleNullable(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetByteArray(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetDateTimeNullable(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetInt32Array(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetInt64Nullable(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetObject(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetRational(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetRationalArray(ExifDirectoryBase.TagAperture));
            Assert.IsNull(_directory.GetStringArray(ExifDirectoryBase.TagAperture));
        }

        [Test]
        public void TestToString()
        {
            Directory directory = new ExifIfd0Directory();
            Assert.AreEqual("Exif IFD0 Directory (0 tags)", directory.ToString());
            directory.Set(1, "Tag 1");
            Assert.AreEqual("Exif IFD0 Directory (1 tag)", directory.ToString());
            directory.Set(2, "Tag 2");
            Assert.AreEqual("Exif IFD0 Directory (2 tags)", directory.ToString());
        }
    }
}
